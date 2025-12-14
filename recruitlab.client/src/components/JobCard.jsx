import React from "react";
import { MapPin, DollarSign, Clock } from "lucide-react";

const JobCard = ({ job }) => {
  return (
    <div className="group bg-white border border-gray-200 rounded-xl p-6 hover:border-blue-300 hover:shadow-lg transition-all duration-200 flex flex-col justify-between h-full">
      <div>
        <div className="flex justify-between items-start mb-3">
          <div>
            <h3 className="text-lg font-bold text-gray-900 group-hover:text-blue-600 transition-colors">
              {job.title}
            </h3>
            <p className="text-sm font-medium text-blue-600">
              {job.department}
            </p>
          </div>
          <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
            Open
          </span>
        </div>

        <p className="text-sm text-gray-600 line-clamp-3 mb-5 leading-relaxed">
          {job.description}
        </p>

        {/* Skills Tags */}
        <div className="flex flex-wrap gap-2 mb-6">
          {job.jobSkills &&
            job.jobSkills.slice(0, 3).map((skill) => (
              <span
                key={skill.id}
                className="inline-flex items-center px-2.5 py-1 rounded-md text-xs font-medium bg-gray-50 text-gray-600 border border-gray-100"
              >
                {skill.skillName}
              </span>
            ))}
          {job.jobSkills && job.jobSkills.length > 3 && (
            <span className="text-xs text-gray-400 py-1 self-center">
              +{job.jobSkills.length - 3} more
            </span>
          )}
        </div>
      </div>

      {/* Footer Info */}
      <div className="pt-4 border-t border-gray-100 flex items-center justify-between mt-auto">
        <div className="flex flex-col gap-1">
          <div className="flex items-center gap-1 text-xs text-gray-500">
            <MapPin className="w-3 h-3" />
            {job.location}
          </div>
          <div className="flex items-center gap-1 text-xs text-gray-500">
            <span className="w-3 h-3 m-2">INR</span>
            {job.salaryMin
              ? `${job.salaryMin} - ${job.salaryMax}`
              : "Salary not disclosed"}
          </div>
        </div>

        <button className="text-sm font-semibold text-blue-600 hover:text-blue-800 flex items-center gap-1">
          Details <span>&rarr;</span>
        </button>
      </div>
    </div>
  );
};

export default JobCard;
