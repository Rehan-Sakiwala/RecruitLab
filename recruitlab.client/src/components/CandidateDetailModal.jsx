import React from "react";
import { X, Mail, Phone, MapPin, ExternalLink } from "lucide-react";

const CandidateDetailModal = ({ candidate, onClose }) => {
  if (!candidate) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
      {/* Modal Container */}
      <div className="bg-white w-full max-w-2xl rounded-lg shadow-xl overflow-hidden flex flex-col max-h-[90vh]">
        {/* Header */}
        <div className="flex justify-between items-center p-6 border-b border-gray-200 bg-gray-50">
          <div className="flex items-center gap-4">
            <div className="h-12 w-12 rounded-full bg-blue-600 flex items-center justify-center text-white font-bold text-xl">
              {candidate.firstName?.[0]}
              {candidate.lastName?.[0]}
            </div>
            <div>
              <h2 className="text-xl font-bold text-gray-900">
                {candidate.firstName} {candidate.lastName}
              </h2>
              <p className="text-sm text-blue-600 font-medium">
                {candidate.currentPosition || "Role not specified"}
              </p>
            </div>
          </div>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 p-1 rounded-full hover:bg-gray-200 transition"
          >
            <X size={24} />
          </button>
        </div>

        {/* Scrollable Content */}
        <div className="p-6 overflow-y-auto space-y-6">
          {/* Contact Details */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="flex items-center gap-3 text-gray-700">
              <Mail size={18} className="text-gray-400" />
              <span>{candidate.email}</span>
            </div>
            <div className="flex items-center gap-3 text-gray-700">
              <Phone size={18} className="text-gray-400" />
              <span>{candidate.phoneNumber || "No phone provided"}</span>
            </div>

            {/* Links */}
            {(candidate.linkedInProfile || candidate.portfolioUrl) && (
              <div className="col-span-full flex gap-4 mt-2">
                {candidate.linkedInProfile && (
                  <a
                    href={candidate.linkedInProfile}
                    target="_blank"
                    rel="noreferrer"
                    className="text-blue-600 hover:underline flex items-center gap-1 text-sm"
                  >
                    <ExternalLink size={14} /> LinkedIn Profile
                  </a>
                )}
                {candidate.portfolioUrl && (
                  <a
                    href={candidate.portfolioUrl}
                    target="_blank"
                    rel="noreferrer"
                    className="text-blue-600 hover:underline flex items-center gap-1 text-sm"
                  >
                    <ExternalLink size={14} /> Portfolio
                  </a>
                )}
              </div>
            )}
          </div>

          <hr className="border-gray-100" />

          {/* Skills */}
          <div>
            <h3 className="font-semibold text-gray-900 mb-3">Skills</h3>
            <div className="flex flex-wrap gap-2">
              {candidate.candidateSkills?.length > 0 ? (
                candidate.candidateSkills.map((skill, index) => (
                  <span
                    key={index}
                    className="px-3 py-1 bg-gray-100 text-gray-700 rounded-md text-sm border border-gray-200"
                  >
                    {skill.skillName}
                  </span>
                ))
              ) : (
                <span className="text-gray-500 text-sm">No skills listed</span>
              )}
            </div>
          </div>

          <hr className="border-gray-100" />

          {/* Experience */}
          <div>
            <h3 className="font-semibold text-gray-900 mb-3">Experience</h3>
            {candidate.experienceHistory?.length > 0 ? (
              <div className="space-y-4">
                {candidate.experienceHistory.map((exp, idx) => (
                  <div
                    key={idx}
                    className="flex flex-col sm:flex-row sm:justify-between sm:items-start gap-1"
                  >
                    <div>
                      <h4 className="font-medium text-gray-900">
                        {exp.position}
                      </h4>
                      <p className="text-sm text-gray-600">{exp.companyName}</p>
                    </div>
                    <span className="text-xs text-gray-500 bg-gray-50 px-2 py-1 rounded">
                      {new Date(exp.startDate).toLocaleDateString()} -{" "}
                      {exp.isCurrent
                        ? "Present"
                        : new Date(exp.endDate).toLocaleDateString()}
                    </span>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-gray-500 text-sm">
                No experience history available.
              </p>
            )}
          </div>
        </div>

        {/* Footer */}
        <div className="p-4 border-t border-gray-200 bg-gray-50 flex justify-end gap-3">
          <button
            onClick={onClose}
            className="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 font-medium text-sm"
          >
            Close
          </button>
          <button
            onClick={() =>
              alert("Job linking functionality will be implemented soon.")
            }
            className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 font-medium text-sm shadow-sm"
          >
            Link to Job
          </button>
        </div>
      </div>
    </div>
  );
};

export default CandidateDetailModal;
