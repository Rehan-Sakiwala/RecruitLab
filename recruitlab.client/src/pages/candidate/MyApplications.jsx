import React, { useEffect, useState } from "react";
import applicationService from "../../services/applicationService";
import { Calendar, Briefcase } from "lucide-react";

const MyApplications = () => {
  const [applications, setApplications] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchApps = async () => {
      try {
        const res = await applicationService.getMyApplications();
        setApplications(res.data);
      } catch (err) {
        console.error("Failed to fetch applications", err);
      } finally {
        setLoading(false);
      }
    };
    fetchApps();
  }, []);

  const getStageBadge = (stage) => {
    // Mapping enum values to text/colors
    // 1=Applied, 2=Screening, 3=Interview, 4=Offer, 5=Hired, 6=Rejected
    const stages = {
      1: { label: "Applied", color: "bg-blue-100 text-blue-800" },
      2: { label: "Screening", color: "bg-yellow-100 text-yellow-800" },
      3: { label: "Interview", color: "bg-purple-100 text-purple-800" },
      4: { label: "Offer", color: "bg-green-100 text-green-800" },
      5: { label: "Hired", color: "bg-green-100 text-green-800" },
      6: { label: "Rejected", color: "bg-red-100 text-red-800" },
      7: { label: "On Hold", color: "bg-gray-100 text-gray-800" },
    };

    const config = stages[stage] || { label: "Unknown", color: "bg-gray-100" };

    return (
      <span
        className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${config.color}`}
      >
        {config.label}
      </span>
    );
  };

  if (loading)
    return (
      <div className="p-10 text-center text-gray-500">
        Loading applications...
      </div>
    );

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-900 mb-6">My Applications</h1>

      {applications.length === 0 ? (
        <div className="text-center py-12 bg-white rounded-lg border border-gray-200">
          <p className="text-gray-500">You haven't applied to any jobs yet.</p>
        </div>
      ) : (
        <div className="bg-white border border-gray-200 rounded-lg overflow-hidden shadow-sm">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Job Role
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Applied Date
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Current Status
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {applications.map((app) => (
                <tr key={app.id}>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="flex items-center">
                      <div className="flex-shrink-0 h-10 w-10 bg-blue-50 rounded-full flex items-center justify-center text-blue-600">
                        <Briefcase size={18} />
                      </div>
                      <div className="ml-4">
                        <div className="text-sm font-medium text-gray-900">
                          {app.jobTitle}
                        </div>
                        <div className="text-sm text-gray-500">
                          Job ID: {app.jobOpeningId}
                        </div>
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="flex items-center text-sm text-gray-500">
                      <Calendar size={16} className="mr-2" />
                      {new Date(app.appliedAt).toLocaleDateString()}
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    {getStageBadge(app.stage)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default MyApplications;
