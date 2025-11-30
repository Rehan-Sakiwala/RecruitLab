import React from "react";
import { useNavigate } from "react-router-dom";
import jobService from "../../../services/jobService";
import JobForm from "../../../components/recruiter/JobForm";
import { ArrowLeft } from "lucide-react";

const CreateJob = () => {
  const navigate = useNavigate();

  const handleCreate = async (jobData) => {
    try {
      await jobService.createJob(jobData);
      // We could use a toast notification here
      navigate("/recruiter/jobs");
    } catch (err) {
      throw err; // Form component will handle error display
    }
  };

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
          <h1 className="text-2xl font-bold text-gray-900">Post a New Job</h1>
          <p className="text-gray-500 text-sm mt-1">
            Define the requirements and skills for the new position.
          </p>
        </div>
      </div>

      <JobForm onSubmit={handleCreate} />
    </div>
  );
};

export default CreateJob;
