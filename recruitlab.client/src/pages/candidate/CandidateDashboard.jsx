import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import applicationService from "../../services/applicationService";
import { Briefcase, CheckCircle, Clock } from "lucide-react";

const CandidateDashboard = () => {
  const [stats, setStats] = useState({ total: 0, active: 0, interviews: 0 });
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const res = await applicationService.getMyApplications();
        const apps = res.data;

        const activeApps = apps.filter((a) => a.stage !== 6 && a.stage !== 5);
        const interviews = apps.filter((a) => a.stage === 3);

        setStats({
          total: apps.length,
          active: activeApps.length,
          interviews: interviews.length,
        });
      } catch (err) {
        console.error("Failed to load dashboard", err);
      } finally {
        setLoading(false);
      }
    };
    fetchStats();
  }, []);

  if (loading)
    return <div className="p-8 text-gray-500">Loading dashboard...</div>;

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Welcome Back!</h1>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div className="bg-white p-6 rounded-xl shadow-sm border border-gray-200 flex items-center">
          <div className="p-3 bg-blue-50 rounded-lg mr-4 text-blue-600">
            <Briefcase size={24} />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-500">
              Total Applications
            </p>
            <p className="text-2xl font-bold text-gray-900">{stats.total}</p>
          </div>
        </div>

        <div className="bg-white p-6 rounded-xl shadow-sm border border-gray-200 flex items-center">
          <div className="p-3 bg-yellow-50 rounded-lg mr-4 text-yellow-600">
            <Clock size={24} />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-500">In Progress</p>
            <p className="text-2xl font-bold text-gray-900">{stats.active}</p>
          </div>
        </div>

        <div className="bg-white p-6 rounded-xl shadow-sm border border-gray-200 flex items-center">
          <div className="p-3 bg-purple-50 rounded-lg mr-4 text-purple-600">
            <CheckCircle size={24} />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-500">Interviews</p>
            <p className="text-2xl font-bold text-gray-900">
              {stats.interviews}
            </p>
          </div>
        </div>
      </div>

      <div className="bg-blue-50 border border-blue-100 rounded-lg p-6">
        <h3 className="text-lg font-semibold text-blue-900 mb-2">
          Ready for your next opportunity?
        </h3>
        <p className="text-blue-700 mb-4">
          Browse our open positions and find the perfect role for you.
        </p>

        {/* UPDATED BUTTON: Navigates to internal jobs page */}
        <button
          onClick={() => navigate("/candidate/jobs")}
          className="inline-block px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
        >
          View Open Jobs
        </button>
      </div>
    </div>
  );
};

export default CandidateDashboard;
