import React, { useEffect, useState } from "react";
import candidateService from "../../services/candidateService";
import CandidateDetailModal from "../../components/CandidateDetailModal"; // 1. Import Modal
import { Search, Loader2 } from "lucide-react";

const Candidates = () => {
  const [candidates, setCandidates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");

  // 2. Add State for Modal
  const [selectedCandidate, setSelectedCandidate] = useState(null);
  const [detailsLoading, setDetailsLoading] = useState(false);

  // Pagination State
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const pageSize = 10;

  useEffect(() => {
    fetchCandidates();
  }, [page, search]);

  const fetchCandidates = async () => {
    try {
      setLoading(true);
      const data = await candidateService.getAllCandidates(
        page,
        pageSize,
        search
      );
      setCandidates(data.candidates || []);
      setTotalPages(Math.ceil((data.totalCount || 0) / pageSize));
    } catch (error) {
      console.error("Failed to load candidates", error);
    } finally {
      setLoading(false);
    }
  };

  // 3. Handle View Click
  const handleViewCandidate = async (id) => {
    try {
      setDetailsLoading(true); // Show some loading indicator if needed
      // Fetch full profile details
      const fullData = await candidateService.getCandidateById(id);
      setSelectedCandidate(fullData);
    } catch (error) {
      alert("Failed to fetch candidate details");
      console.error(error);
    } finally {
      setDetailsLoading(false);
    }
  };

  const handleSearch = (e) => {
    setSearch(e.target.value);
    setPage(1);
  };

  return (
    <div className="bg-white border border-gray-200 rounded-xl shadow-sm overflow-hidden min-h-[500px]">
      {/* 4. Render Modal Conditionally */}
      {selectedCandidate && (
        <CandidateDetailModal
          candidate={selectedCandidate}
          onClose={() => setSelectedCandidate(null)}
        />
      )}

      {/* Header */}
      <div className="p-6 border-b border-gray-100 flex justify-between items-center">
        <div>
          <h2 className="text-xl font-bold text-gray-900">
            Candidates Database
          </h2>
          <p className="text-sm text-gray-500">
            View and manage potential hires.
          </p>
        </div>

        <div className="relative">
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Search className="h-4 w-4 text-gray-400" />
          </div>
          <input
            type="text"
            className="pl-9 pr-4 py-2 border border-gray-300 rounded-lg text-sm focus:ring-blue-500 focus:border-blue-500 w-64"
            placeholder="Search candidates..."
            value={search}
            onChange={handleSearch}
          />
        </div>
      </div>

      {/* Table */}
      {loading ? (
        <div className="p-20 text-center flex justify-center">
          <Loader2 className="animate-spin text-blue-600" />
        </div>
      ) : (
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Candidate
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Role & Company
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Top Skills
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Status
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                Action
              </th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {candidates.length === 0 ? (
              <tr>
                <td
                  colSpan="5"
                  className="px-6 py-10 text-center text-gray-500"
                >
                  No candidates found.
                </td>
              </tr>
            ) : (
              candidates.map((c) => (
                <tr key={c.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="flex items-center">
                      <div className="h-9 w-9 rounded-full bg-blue-100 flex items-center justify-center text-blue-700 font-bold text-sm uppercase">
                        {c.firstName?.[0]}
                        {c.lastName?.[0]}
                      </div>
                      <div className="ml-3">
                        <div className="text-sm font-medium text-gray-900">
                          {c.firstName} {c.lastName}
                        </div>
                        <div className="text-xs text-gray-500">{c.email}</div>
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="text-sm text-gray-900">
                      {c.currentPosition || "N/A"}
                    </div>
                    <div className="text-xs text-gray-500">
                      {c.currentCompany}
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <div className="flex flex-wrap gap-1">
                      {c.skills?.slice(0, 2).map((skill, i) => (
                        <span
                          key={i}
                          className="px-2 py-0.5 rounded text-[10px] font-medium bg-gray-100 text-gray-600 border border-gray-200"
                        >
                          {skill}
                        </span>
                      ))}
                      {c.skills?.length > 2 && (
                        <span className="text-[10px] text-gray-400 self-center">
                          +{c.skills.length - 2}
                        </span>
                      )}
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span
                      className={`px-2 py-0.5 inline-flex text-xs font-medium rounded-full ${
                        c.isAvailable
                          ? "bg-green-50 text-green-700 border border-green-100"
                          : "bg-gray-100 text-gray-600"
                      }`}
                    >
                      {c.isAvailable ? "Available" : "Hired"}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button
                      onClick={() => handleViewCandidate(c.id)}
                      disabled={detailsLoading}
                      className="text-blue-600 hover:text-blue-900 text-xs font-semibold border border-blue-200 px-3 py-1 rounded hover:bg-blue-50 transition-colors disabled:opacity-50"
                    >
                      {detailsLoading ? "..." : "View"}
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default Candidates;
