import React, { useEffect, useState } from "react";
import jobService from "../../services/jobService";
import applicationService from "../../services/applicationService";
import {
  MapPin,
  DollarSign,
  Briefcase,
  CheckCircle,
  Loader2,
} from "lucide-react";

const CandidateJobs = () => {
  const [jobs, setJobs] = useState([]);
  const [myApplications, setMyApplications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [applyingId, setApplyingId] = useState(null);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [jobsRes, appsRes] = await Promise.all([
        jobService.getAllJobs(),
        applicationService.getMyApplications(),
      ]);

      // Filter only Open jobs
      const openJobs = jobsRes.data.filter((j) => j.statusName === "Open");
      setJobs(openJobs);
      setMyApplications(appsRes.data);
    } catch (err) {
      console.error("Failed to load jobs", err);
    } finally {
      setLoading(false);
    }
  };

  const handleApply = async (jobId) => {
    try {
      setApplyingId(jobId);
      await applicationService.applyForJob(jobId);

      const appsRes = await applicationService.getMyApplications();
      setMyApplications(appsRes.data);
      alert("Application submitted successfully!");
    } catch (err) {
      alert("Failed to apply: " + (err.response?.data?.message || err.message));
    } finally {
      setApplyingId(null);
    }
  };

  const hasApplied = (jobId) => {
    return myApplications.some((app) => app.jobOpeningId === jobId);
  };

  if (loading)
    return (
      <div className="p-10 text-center text-gray-500">
        Loading opportunities...
      </div>
    );

  return (
    <div className="max-w-6xl mx-auto space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            Open Opportunities
          </h1>
          <p className="text-gray-500 text-sm mt-1">
            Find and apply for roles that match your skills.
          </p>
        </div>
        <div className="text-sm text-gray-500">Showing {jobs.length} jobs</div>
      </div>

      <div className="grid grid-cols-1 gap-4">
        {jobs.map((job) => {
          const applied = hasApplied(job.id);

          return (
            <div
              key={job.id}
              className="bg-white border border-gray-200 rounded-lg p-6 hover:shadow-md transition-shadow flex flex-col md:flex-row justify-between gap-6"
            >
              <div className="flex-1">
                <div className="flex items-start justify-between mb-2">
                  <div>
                    <h3 className="text-lg font-bold text-gray-900">
                      {job.title}
                    </h3>
                    <p className="text-blue-600 text-sm font-medium">
                      {job.department}
                    </p>
                  </div>
                  {applied && (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                      <CheckCircle size={14} className="mr-1" /> Applied
                    </span>
                  )}
                </div>

                <p className="text-gray-600 text-sm mb-4 line-clamp-2">
                  {job.description}
                </p>

                <div className="flex flex-wrap gap-4 text-sm text-gray-500">
                  <div className="flex items-center">
                    <MapPin size={16} className="mr-1" /> {job.location}
                  </div>
                  <div className="flex items-center">
                    <DollarSign size={16} className="mr-1" />
                    {job.salaryMin
                      ? `${job.salaryMin} - ${job.salaryMax}`
                      : "Salary Disclosed"}
                  </div>
                  <div className="flex items-center">
                    <Briefcase size={16} className="mr-1" />{" "}
                    {new Date(job.createdAt).toLocaleDateString()}
                  </div>
                </div>

                <div className="mt-4 flex flex-wrap gap-2">
                  {job.jobSkills.map((skill) => (
                    <span
                      key={skill.id}
                      className="bg-gray-100 text-gray-700 px-2 py-1 rounded text-xs"
                    >
                      {skill.skillName}
                    </span>
                  ))}
                </div>
              </div>

              <div className="flex items-center md:self-center">
                <button
                  onClick={() => handleApply(job.id)}
                  disabled={applied || applyingId === job.id}
                  className={`px-6 py-2.5 rounded-lg text-sm font-medium transition-all w-full md:w-auto min-w-[120px] flex justify-center items-center ${
                    applied
                      ? "bg-gray-100 text-gray-400 cursor-not-allowed"
                      : "bg-blue-600 text-white hover:bg-blue-700 shadow-sm"
                  }`}
                >
                  {applyingId === job.id ? (
                    <Loader2 size={18} className="animate-spin" />
                  ) : applied ? (
                    "Applied"
                  ) : (
                    "Apply Now"
                  )}
                </button>
              </div>
            </div>
          );
        })}

        {jobs.length === 0 && (
          <div className="text-center py-20 bg-white rounded-lg border border-gray-200">
            <p className="text-gray-500">
              No open positions found at the moment.
            </p>
          </div>
        )}
      </div>
    </div>
  );
};

export default CandidateJobs;
