import React from "react";
import {
  ArrowLeft,
  MapPin,
  DollarSign,
  IndianRupee,
  Clock,
  Briefcase,
  CheckCircle,
} from "lucide-react";

const JobDetailModal = ({ job, onClose }) => {
  if (!job) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 sm:p-6">
      {/* Backdrop with blur */}
      <div
        className="absolute inset-0 bg-gray-900/40 backdrop-blur-sm transition-opacity"
        onClick={onClose}
      />

      {/* Content */}
      <div className="relative bg-white rounded-2xl shadow-2xl w-full max-w-4xl max-h-[90vh] overflow-hidden flex flex-col animate-in fade-in zoom-in-95 duration-200">
        {/* Header */}
        <div className="flex items-center gap-4 p-6 border-b border-gray-100 bg-white z-10">
          <button
            onClick={onClose}
            className="p-2 hover:bg-gray-100 rounded-full transition-colors text-gray-500 hover:text-gray-900"
          >
            <ArrowLeft className="w-6 h-6" />
          </button>
          <span className="text-sm font-semibold text-gray-400 uppercase tracking-wider">
            Job Details
          </span>
        </div>

        {/* Scrollable Content */}
        <div className="overflow-y-auto p-6 sm:p-10">
          <div className="flex flex-col sm:flex-row justify-between items-start gap-6 mb-8">
            <div>
              <h1 className="text-3xl font-bold text-gray-900 mb-2">
                {job.title}
              </h1>
              <p className="text-xl text-blue-600 font-medium">
                {job.department}
              </p>
            </div>
            <span className="px-4 py-2 bg-green-100 text-green-800 rounded-full text-sm font-bold">
              Open Position
            </span>
          </div>

          {/* Information Grid */}
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-10">
            <div className="flex items-center gap-3 p-4 bg-gray-50 rounded-xl">
              <MapPin className="text-blue-500" />
              <div>
                <p className="text-xs text-gray-500 uppercase">Location</p>
                <p className="font-semibold text-gray-900">{job.location}</p>
              </div>
            </div>
            <div className="flex items-center gap-3 p-4 bg-gray-50 rounded-xl">
              <IndianRupee className="text-green-500" />
              <div>
                <p className="text-xs text-gray-500 uppercase">Salary</p>
                <p className="font-semibold text-gray-900">
                  {job.salaryMin
                    ? `${job.salaryMin} - ${job.salaryMax}`
                    : "Not Disclosed"}
                </p>
              </div>
            </div>
            <div className="flex items-center gap-3 p-4 bg-gray-50 rounded-xl">
              <Briefcase className="text-purple-500" />
              <div>
                <p className="text-xs text-gray-500 uppercase">Type</p>
                <p className="font-semibold text-gray-900">Full Time</p>
              </div>
            </div>
          </div>

          <div className="space-y-8">
            <section>
              <h3 className="text-xl font-bold text-gray-900 mb-4">
                About the Role
              </h3>
              <p className="text-gray-600 leading-relaxed text-lg whitespace-pre-line">
                {job.description}
              </p>
            </section>

            <section>
              <h3 className="text-xl font-bold text-gray-900 mb-4">
                Required Skills
              </h3>
              <div className="flex flex-wrap gap-2">
                {job.jobSkills?.map((skill) => (
                  <span
                    key={skill.id}
                    className="flex items-center gap-2 px-4 py-2 bg-blue-50 text-blue-700 rounded-lg font-medium border border-blue-100"
                  >
                    <CheckCircle className="w-4 h-4" />
                    {skill.skillName}
                  </span>
                ))}
              </div>
            </section>
          </div>
        </div>

        {/* Footer(Buttons) */}
        <div className="p-6 border-t border-gray-100 bg-gray-50 flex justify-end gap-4">
          <button
            onClick={onClose}
            className="px-6 py-3 text-gray-700 font-semibold hover:bg-gray-200 rounded-lg transition-colors"
          >
            Cancel
          </button>
          <button
            className="px-8 py-3 bg-blue-600 text-white font-bold rounded-lg hover:bg-blue-700 shadow-lg shadow-blue-200 transition-all transform hover:-translate-y-0.5"
            onClick={() => alert("Redirect to Login/Register to apply")}
          >
            Apply Now
          </button>
        </div>
      </div>
    </div>
  );
};

export default JobDetailModal;
