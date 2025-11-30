import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import jobService from "../../../services/jobService";
import JobForm from "../../../components/recruiter/JobForm";
import { ArrowLeft } from "lucide-react";

const EditJob = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [jobData, setJobData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchJob = async () => {
      try {
        const res = await jobService.getJobById(id);
        setJobData(res.data);
      } catch (err) {
        console.error("Error fetching job", err);
        navigate("/recruiter/jobs");
      } finally {
        setLoading(false);
      }
    };
    fetchJob();
  }, [id, navigate]);

  const handleUpdate = async (updatedData) => {
    try {
      await jobService.updateJob(id, updatedData);
      navigate("/recruiter/jobs");
    } catch (err) {
      throw err;
    }
  };

  if (loading)
    return (
      <div className="p-10 text-center text-gray-500">
        Loading job details...
      </div>
    );

  return (
    <div className="max-w-5xl mx-auto">
      <div className="mb-8 flex items-center gap-4">
        <button
          onClick={() => navigate("/recruiter/jobs")}
          className="p-2 hover:bg-white rounded-full transition-colors border border-transparent hover:border-gray-200"
        >
          <ArrowLeft size={20} className="text-gray-600" />
        </button>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            Edit Job: {jobData?.title}
          </h1>
          <p className="text-gray-500 text-sm mt-1">
            Update job details, skills requirements, or status.
          </p>
        </div>
      </div>

      <JobForm initialData={jobData} onSubmit={handleUpdate} isEdit={true} />
    </div>
  );
};

export default EditJob;
