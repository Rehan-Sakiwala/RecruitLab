import React, { useEffect, useState } from "react";
import jobService from "../../services/jobService";
import applicationService from "../../services/applicationService";
import ScheduleInterviewModal from "../../components/ScheduleInterviewModal";
import { Search, Loader2, MapPin, ChevronRight, Filter } from "lucide-react";

const Applications = () => {
  const [jobs, setJobs] = useState([]);
  const [selectedJob, setSelectedJob] = useState(null);
  const [applications, setApplications] = useState([]);

  const [loadingJobs, setLoadingJobs] = useState(true);
  const [loadingApps, setLoadingApps] = useState(false);
  const [search, setSearch] = useState("");

  const [scheduleModalOpen, setScheduleModalOpen] = useState(false);
  const [selectedAppForSchedule, setSelectedAppForSchedule] = useState(null);

  useEffect(() => {
    fetchJobs();
  }, []);
  useEffect(() => {
    if (selectedJob?.id) fetchApplications(selectedJob.id);
  }, [selectedJob]);

  const fetchJobs = async () => {
    try {
      setLoadingJobs(true);
      const response = await jobService.getAllJobs();

      let list = [];
      if (Array.isArray(response)) list = response;
      else if (response && Array.isArray(response.jobs)) list = response.jobs;
      else if (response && Array.isArray(response.data)) list = response.data;

      setJobs(list);
      if (list.length > 0 && !selectedJob) setSelectedJob(list[0]);
    } catch (error) {
      console.error("Error fetching jobs:", error);
    } finally {
      setLoadingJobs(false);
    }
  };

  const fetchApplications = async (jobId) => {
    try {
      setLoadingApps(true);
      const response = await applicationService.getApplicationsForJob(jobId);

      let list = [];
      if (Array.isArray(response)) list = response;
      else if (response && Array.isArray(response.applications))
        list = response.applications;
      else if (response && Array.isArray(response.data)) list = response.data;

      setApplications(list);
    } catch (error) {
      console.error("Error fetching applications:", error);
      setApplications([]);
    } finally {
      setLoadingApps(false);
    }
  };

  // Open Modal Handler
  const handleOpenSchedule = (app) => {
    setSelectedAppForSchedule(app);
    setScheduleModalOpen(true);
  };

  const getStatusStyle = (status) => {
    switch (status) {
      case "Hired":
        return "bg-green-100 text-green-700";
      case "Rejected":
        return "bg-red-100 text-red-700";
      case "Interview":
        return "bg-purple-100 text-purple-700";
      case "Screening":
        return "bg-blue-100 text-blue-700";
      case "Offer":
        return "bg-yellow-100 text-yellow-800";
      default:
        return "bg-gray-100 text-gray-700";
    }
  };

  const filteredJobs = jobs.filter((j) =>
    j.title?.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="flex h-[calc(100vh-7rem)] border border-gray-200 rounded-lg bg-white shadow-sm overflow-hidden">
      {/* --- SCHEDULE INTERVIEW MODAL --- */}
      {scheduleModalOpen && selectedAppForSchedule && (
        <ScheduleInterviewModal
          applicationId={selectedAppForSchedule.id}
          candidateName={selectedAppForSchedule.candidateName || "Candidate"}
          onClose={() => setScheduleModalOpen(false)}
          onSuccess={() => {
            // Refresh list to update status to 'Interview'
            if (selectedJob?.id) fetchApplications(selectedJob.id);
          }}
        />
      )}

      {/* LEFT: Jobs Sidebar */}
      <div className="w-80 border-r border-gray-200 flex flex-col bg-gray-50">
        <div className="p-4 border-b border-gray-200 bg-white">
          <h2 className="font-bold text-gray-800">Jobs</h2>
          <div className="mt-2 relative">
            <Search
              className="absolute left-3 top-2.5 text-gray-400"
              size={16}
            />
            <input
              className="w-full pl-9 pr-3 py-2 text-sm border border-gray-300 rounded-md focus:outline-none focus:border-blue-500"
              placeholder="Search jobs..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
        </div>

        <div className="overflow-y-auto flex-1">
          {loadingJobs ? (
            <div className="p-8 flex justify-center">
              <Loader2 className="animate-spin text-blue-500" />
            </div>
          ) : filteredJobs.length === 0 ? (
            <div className="p-6 text-center text-gray-400 text-sm">
              No jobs found.
            </div>
          ) : (
            filteredJobs.map((job) => (
              <div
                key={job.id}
                onClick={() => setSelectedJob(job)}
                className={`p-4 border-b border-gray-100 cursor-pointer hover:bg-white transition-colors ${
                  selectedJob?.id === job.id
                    ? "bg-white border-l-4 border-l-blue-600"
                    : ""
                }`}
              >
                <div className="flex justify-between items-center">
                  <h3
                    className={`text-sm font-semibold ${
                      selectedJob?.id === job.id
                        ? "text-blue-700"
                        : "text-gray-700"
                    }`}
                  >
                    {job.title}
                  </h3>
                  {selectedJob?.id === job.id && (
                    <ChevronRight size={14} className="text-blue-500" />
                  )}
                </div>
                <div className="flex items-center gap-1 mt-1 text-xs text-gray-500">
                  <MapPin size={12} /> {job.location}
                </div>
              </div>
            ))
          )}
        </div>
      </div>

      {/* RIGHT: Applications Table */}
      <div className="flex-1 flex flex-col">
        <div className="h-16 border-b border-gray-200 flex items-center justify-between px-6 bg-white">
          <h2 className="text-lg font-bold text-gray-800">
            {selectedJob ? selectedJob.title : "Select a Job"}
          </h2>
          <span className="bg-blue-50 text-blue-700 text-xs font-semibold px-3 py-1 rounded-full border border-blue-100">
            {applications.length} Applicants
          </span>
        </div>

        <div className="flex-1 overflow-y-auto bg-gray-50/50 p-0">
          {loadingApps ? (
            <div className="h-full flex items-center justify-center">
              <Loader2 className="animate-spin text-blue-500" size={32} />
            </div>
          ) : applications.length === 0 ? (
            <div className="h-full flex flex-col items-center justify-center text-gray-400">
              <Filter size={40} className="mb-2 opacity-20" />
              <p>No applications found.</p>
            </div>
          ) : (
            <table className="w-full text-left border-collapse">
              <thead className="bg-gray-50 sticky top-0 shadow-sm text-xs uppercase text-gray-500 font-semibold">
                <tr>
                  <th className="px-6 py-3">Candidate</th>
                  <th className="px-6 py-3">Applied</th>
                  <th className="px-6 py-3">Stage</th>
                  <th className="px-6 py-3 text-right">Action</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-100 text-sm">
                {applications.map((app) => (
                  <tr
                    key={app.id}
                    className="hover:bg-blue-50/30 transition-colors"
                  >
                    <td className="px-6 py-4">
                      <div className="flex items-center">
                        <div className="h-8 w-8 rounded-full bg-blue-100 text-blue-600 flex items-center justify-center font-bold text-xs mr-3">
                          {(app.candidateName || "U")[0]}
                        </div>
                        <div>
                          <p className="font-medium text-gray-900">
                            {app.candidateName || "Unknown"}
                          </p>
                          <p className="text-xs text-gray-500">
                            {app.candidateEmail}
                          </p>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4 text-gray-500">
                      {new Date(app.appliedAt).toLocaleDateString()}
                    </td>
                    <td className="px-6 py-4">
                      <span
                        className={`px-2 py-1 rounded text-xs font-medium ${getStatusStyle(
                          app.stage || app.status
                        )}`}
                      >
                        {app.stage || app.status || "Applied"}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-right">
                      <button
                        onClick={() => handleOpenSchedule(app)}
                        className="text-blue-600 hover:text-blue-800 font-medium text-xs border border-blue-100 hover:bg-blue-50 px-3 py-1.5 rounded transition"
                      >
                        Schedule
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </div>
  );
};

export default Applications;
