import React, { useEffect, useState } from "react";
import axios from "axios";
import { getAuthHeaders } from "../../utils/authHeaders";
import { Briefcase, Users, Calendar } from "lucide-react";

const RecruiterDashboard = () => {
  const [stats, setStats] = useState({ openJobs: 0, totalJobs: 0 });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchDashboardData = async () => {
      try {
        // Fetch jobs to calculate stats
        const jobRes = await axios.get(
          "https://localhost:7100/api/Job",
          getAuthHeaders()
        );

        const allJobs = jobRes.data;
        const openJobs = allJobs.filter((j) => j.statusName === "Open").length;

        setStats({
          openJobs: openJobs,
          totalJobs: allJobs.length,
        });
      } catch (error) {
        console.error("Error fetching dashboard data", error);
      } finally {
        setLoading(false);
      }
    };

    fetchDashboardData();
  }, []);

  if (loading)
    return <div className="p-8 text-gray-500">Loading dashboard...</div>;

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Overview</h1>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {/* Stat Card 1 */}
        <div className="bg-white p-6 rounded-xl shadow-sm border border-gray-200 flex items-center">
          <div className="p-3 bg-blue-50 rounded-lg mr-4 text-blue-600">
            <Briefcase size={24} />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-500">Open Jobs</p>
            <p className="text-2xl font-bold text-gray-900">{stats.openJobs}</p>
          </div>
        </div>

        {/* Stat Card 2 */}
        <div className="bg-white p-6 rounded-xl shadow-sm border border-gray-200 flex items-center">
          <div className="p-3 bg-green-50 rounded-lg mr-4 text-green-600">
            <Users size={24} />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-500">
              Total Jobs Posted
            </p>
            <p className="text-2xl font-bold text-gray-900">
              {stats.totalJobs}
            </p>
          </div>
        </div>

        {/* Stat Card 3 (Placeholder for now) */}
        <div className="bg-white p-6 rounded-xl shadow-sm border border-gray-200 flex items-center">
          <div className="p-3 bg-purple-50 rounded-lg mr-4 text-purple-600">
            <Calendar size={24} />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-500">
              Interviews Today
            </p>
            <p className="text-2xl font-bold text-gray-900">0</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default RecruiterDashboard;
