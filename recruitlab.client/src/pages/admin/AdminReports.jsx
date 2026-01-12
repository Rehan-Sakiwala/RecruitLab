import React, { useEffect, useState } from "react";
import jobService from "../../services/jobService";
import userService from "../../services/userService";
import {
  BarChart3,
  Users,
  Briefcase,
  Loader2,
  MapPin,
  Shield,
} from "lucide-react";

const AdminReports = () => {
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState({
    totalJobs: 0,
    jobsByLocation: {},
    totalStaff: 0,
    staffByRole: {},
  });

  useEffect(() => {
    generateReports();
  }, []);

  const generateReports = async () => {
    try {
      setLoading(true);

      const [jobsData, staffData] = await Promise.all([
        jobService.getAllJobs().catch(() => []),
        userService.getStaff().catch(() => []),
      ]);

      const jobs = Array.isArray(jobsData)
        ? jobsData
        : jobsData.jobs || jobsData.data || [];
      const staff = Array.isArray(staffData) ? staffData : staffData.data || [];

      const jobsLocMap = {};
      jobs.forEach((j) => {
        const loc = j.location || "Unknown";
        jobsLocMap[loc] = (jobsLocMap[loc] || 0) + 1;
      });

      const staffRoleMap = {};
      staff.forEach((u) => {
        const role = u.role || "User";
        staffRoleMap[role] = (staffRoleMap[role] || 0) + 1;
      });

      setStats({
        totalJobs: jobs.length,
        jobsByLocation: jobsLocMap,
        totalStaff: staff.length,
        staffByRole: staffRoleMap,
      });
    } catch (error) {
      console.error("Failed to generate reports", error);
    } finally {
      setLoading(false);
    }
  };

  const getPercent = (val, total) =>
    total === 0 ? 0 : Math.round((val / total) * 100);

  if (loading)
    return (
      <div className="p-20 flex justify-center">
        <Loader2 className="animate-spin text-blue-600" size={32} />
      </div>
    );

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">System Reports</h1>
          <p className="text-gray-500">
            Overview of Jobs and Staff allocation.
          </p>
        </div>
        <button
          onClick={generateReports}
          className="text-sm text-blue-600 hover:underline"
        >
          Refresh Data
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <StatCard
          icon={<Briefcase />}
          title="Total Active Jobs"
          value={stats.totalJobs}
          color="bg-blue-50 text-blue-600"
        />
        <StatCard
          icon={<Users />}
          title="Total Staff Members"
          value={stats.totalStaff}
          color="bg-green-50 text-green-600"
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white p-6 rounded-xl border border-gray-200 shadow-sm">
          <h3 className="font-bold text-gray-800 mb-4 flex items-center gap-2">
            <MapPin size={18} /> Jobs by Location
          </h3>
          <div className="space-y-4">
            {Object.keys(stats.jobsByLocation).length === 0 ? (
              <p className="text-gray-400 text-sm">No job data available.</p>
            ) : (
              Object.entries(stats.jobsByLocation).map(([loc, count]) => (
                <div key={loc}>
                  <div className="flex justify-between text-sm mb-1">
                    <span className="text-gray-700 font-medium">{loc}</span>
                    <span className="text-gray-500">
                      {count} ({getPercent(count, stats.totalJobs)}%)
                    </span>
                  </div>
                  <div className="w-full bg-gray-100 rounded-full h-2">
                    <div
                      className="bg-blue-500 h-2 rounded-full"
                      style={{
                        width: `${getPercent(count, stats.totalJobs)}%`,
                      }}
                    ></div>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>

        <div className="bg-white p-6 rounded-xl border border-gray-200 shadow-sm">
          <h3 className="font-bold text-gray-800 mb-4 flex items-center gap-2">
            <Shield size={18} /> Staff Distribution
          </h3>
          <div className="space-y-4">
            {Object.keys(stats.staffByRole).length === 0 ? (
              <p className="text-gray-400 text-sm">No staff data available.</p>
            ) : (
              Object.entries(stats.staffByRole).map(([role, count]) => (
                <div key={role}>
                  <div className="flex justify-between text-sm mb-1">
                    <span className="text-gray-700 font-medium">{role}</span>
                    <span className="text-gray-500">
                      {count} ({getPercent(count, stats.totalStaff)}%)
                    </span>
                  </div>
                  <div className="w-full bg-gray-100 rounded-full h-2">
                    <div
                      className={`h-2 rounded-full ${
                        role === "Admin"
                          ? "bg-red-500"
                          : role === "Recruiter"
                          ? "bg-blue-500"
                          : role === "Interviewer"
                          ? "bg-purple-500"
                          : "bg-green-500"
                      }`}
                      style={{
                        width: `${getPercent(count, stats.totalStaff)}%`,
                      }}
                    ></div>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

const StatCard = ({ icon, title, value, label, color }) => (
  <div className="bg-white p-6 rounded-xl border border-gray-200 shadow-sm flex items-center gap-5">
    <div className={`p-4 rounded-lg ${color}`}>
      {React.cloneElement(icon, { size: 28 })}
    </div>
    <div>
      <p className="text-sm font-medium text-gray-500">{title}</p>
      <div className="flex items-baseline gap-1">
        <h4 className="text-3xl font-bold text-gray-900">{value}</h4>
        {label && <span className="text-xs text-gray-400">{label}</span>}
      </div>
    </div>
  </div>
);

export default AdminReports;
