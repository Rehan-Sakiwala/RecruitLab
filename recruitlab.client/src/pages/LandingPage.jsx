import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchOpenJobs, setCurrentPage } from "../features/jobs/jobSlice";
import Navbar from "../components/Navbar";
import JobCard from "../components/JobCard";
import { Users, Zap, Heart, Search, X } from "lucide-react";
import JobDetailModal from "../components/JobDetailModal";

const LandingPage = () => {
  const dispatch = useDispatch();
  const [selectedJob, setSelectedJob] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");

  const {
    list: jobs,
    loading,
    error,
    currentPage,
    jobsPerPage,
  } = useSelector((state) => state.jobs);

  useEffect(() => {
    dispatch(fetchOpenJobs());
  }, [dispatch]);

  // Search Functionality
  const filteredJobs = jobs.filter((job) => {
    if (!searchTerm) return true;
    const term = searchTerm.toLowerCase();

    // Check Title, Department, Location
    const basicMatch =
      job.title?.toLowerCase().includes(term) ||
      job.department?.toLowerCase().includes(term) ||
      job.location?.toLowerCase().includes(term);

    // Check Skills (Nested Array)
    const skillMatch = job.jobSkills?.some((skill) =>
      skill.skillName?.toLowerCase().includes(term)
    );

    return basicMatch || skillMatch;
  });

  // Pagination Calculation
  const indexOfLastJob = currentPage * jobsPerPage;
  const indexOfFirstJob = indexOfLastJob - jobsPerPage;
  const currentJobs = filteredJobs.slice(indexOfFirstJob, indexOfLastJob);
  const totalPages = Math.ceil(jobs.length / jobsPerPage);

  const handlePageChange = (pageNumber) => {
    dispatch(setCurrentPage(pageNumber));
    const element = document.getElementById("job-section");
    if (element) element.scrollIntoView({ behavior: "smooth" });
  };

  return (
    <div className="min-h-screen bg-gray-50 font-sans text-gray-900">
      <Navbar />

      <JobDetailModal job={selectedJob} onClose={() => setSelectedJob(null)} />

      {/* Description */}
      <div className="bg-white border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pt-20 pb-20 text-center">
          <h1 className="text-5xl sm:text-6xl font-extrabold tracking-tight text-gray-900 mb-6">
            Build the future <br />
            <span className="text-blue-600 relative">
              with RecruitLab
              <svg
                className="absolute w-full h-3 -bottom-1 left-0 text-blue-200 -z-10"
                viewBox="0 0 100 10"
                preserveAspectRatio="none"
              >
                <path
                  d="M0 5 Q 50 10 100 5"
                  stroke="currentColor"
                  strokeWidth="8"
                  fill="none"
                />
              </svg>
            </span>
          </h1>
          <p className="max-w-2xl mx-auto text-xl text-gray-500 mb-10 leading-relaxed">
            We are a team of innovators, creators, and problem solvers. Join us
            in our mission to transform the industry. Your best work starts
            here.
          </p>

          <div className="flex justify-center gap-4">
            <button
              onClick={() =>
                document
                  .getElementById("job-section")
                  .scrollIntoView({ behavior: "smooth" })
              }
              className="px-8 py-3 bg-blue-600 text-white rounded-lg font-semibold hover:bg-blue-700 transition-colors shadow-lg shadow-blue-200"
            >
              View Open Roles
            </button>
            <button className="px-8 py-3 bg-white text-gray-700 border border-gray-300 rounded-lg font-semibold hover:bg-gray-50 transition-colors">
              Life at RecruitLab
            </button>
          </div>
        </div>
      </div>

      {/* Join us */}
      <div className="bg-white border-b border-gray-200 py-12">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8 text-center">
            <div className="p-4">
              <div className="bg-blue-50 w-12 h-12 rounded-full flex items-center justify-center mx-auto mb-4 text-blue-600">
                <Zap className="w-6 h-6" />
              </div>
              <h3 className="font-bold text-lg mb-2">Impact</h3>
              <p className="text-gray-500">
                Work on projects that matter and touch millions of lives.
              </p>
            </div>
            <div className="p-4">
              <div className="bg-green-50 w-12 h-12 rounded-full flex items-center justify-center mx-auto mb-4 text-green-600">
                <Users className="w-6 h-6" />
              </div>
              <h3 className="font-bold text-lg mb-2">Culture</h3>
              <p className="text-gray-500">
                Collaborative, inclusive, and driven by curiosity.
              </p>
            </div>
            <div className="p-4">
              <div className="bg-purple-50 w-12 h-12 rounded-full flex items-center justify-center mx-auto mb-4 text-purple-600">
                <Heart className="w-6 h-6" />
              </div>
              <h3 className="font-bold text-lg mb-2">Benefits</h3>
              <p className="text-gray-500">
                Competitive compensation, health, and wellness perks.
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Jobs Section */}
      <div
        id="job-section"
        className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16"
      >
        <div className="flex flex-col md:flex-row justify-between items-end mb-8 gap-4">
          <div>
            <h2 className="text-3xl font-bold text-gray-900">
              Current Openings
            </h2>
            <p className="text-gray-500 mt-2">
              Join our growing team. We currently have{" "}
              <span className="font-semibold text-gray-900">
                {filteredJobs.length}
              </span>{" "}
              matching positions.
            </p>
          </div>

          {/* Search Input */}
          <div className="relative w-full md:w-96">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Search className="h-5 w-5 text-gray-400" />
            </div>
            <input
              type="text"
              className="block w-full pl-10 pr-10 py-3 border border-gray-300 rounded-lg leading-5 bg-white placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-1 focus:ring-blue-500 focus:border-blue-500 sm:text-sm transition-shadow shadow-sm"
              placeholder="Search by role, skill, or location..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
            {searchTerm && (
              <button
                onClick={() => setSearchTerm("")}
                className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-600"
              >
                <X className="h-5 w-5" />
              </button>
            )}
          </div>
        </div>

        {loading && (
          <div className="flex flex-col items-center justify-center py-20">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mb-4"></div>
            <p className="text-gray-500">Loading opportunities...</p>
          </div>
        )}

        {error && (
          <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-center text-red-700 my-10">
            <p>Unable to load jobs. Please try again later.</p>
          </div>
        )}

        {!loading && !error && filteredJobs.length === 0 && (
          <div className="text-center py-20 bg-white rounded-xl border border-gray-200 border-dashed">
            <p className="text-gray-500 text-lg">
              No jobs found matching "{searchTerm}"
            </p>
            <button
              onClick={() => setSearchTerm("")}
              className="mt-4 text-blue-600 font-medium hover:underline"
            >
              Clear search
            </button>
          </div>
        )}

        {!loading && !error && filteredJobs.length > 0 && (
          <>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
              {currentJobs.map((job) => (
                <JobCard
                  key={job.id}
                  job={job}
                  onDetailsClick={() => setSelectedJob(job)}
                />
              ))}
            </div>

            {/* Pagination Controls */}
            {totalPages > 1 && (
              <div className="flex justify-center mt-16 gap-2">
                <button
                  onClick={() => handlePageChange(currentPage - 1)}
                  disabled={currentPage === 1}
                  className="px-4 py-2 border border-gray-300 rounded-lg text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
                >
                  Previous
                </button>
                {[...Array(totalPages)].map((_, idx) => (
                  <button
                    key={idx + 1}
                    onClick={() => handlePageChange(idx + 1)}
                    className={`w-10 h-10 flex items-center justify-center rounded-lg text-sm font-medium transition-all ${
                      currentPage === idx + 1
                        ? "bg-blue-600 text-white shadow-md transform scale-105"
                        : "bg-white border border-gray-300 text-gray-700 hover:bg-gray-50"
                    }`}
                  >
                    {idx + 1}
                  </button>
                ))}
                <button
                  onClick={() => handlePageChange(currentPage + 1)}
                  disabled={currentPage === totalPages}
                  className="px-4 py-2 border border-gray-300 rounded-lg text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
                >
                  Next
                </button>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
};

export default LandingPage;
