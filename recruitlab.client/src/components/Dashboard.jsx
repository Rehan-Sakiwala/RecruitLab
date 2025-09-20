import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { Link } from 'react-router-dom';
import api from '../services/api';

const Dashboard = () => {
  const { user, isAdmin, isRecruiter } = useAuth();
  const [stats, setStats] = useState({
    totalJobs: 0,
    openJobs: 0,
    onHoldJobs: 0,
    closedJobs: 0,
    myJobs: 0
  });
  const [recentJobs, setRecentJobs] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      const [allJobsResponse, openJobsResponse, onHoldJobsResponse, closedJobsResponse] = await Promise.all([
        api.get('/job'),
        api.get('/job/status/1'),
        api.get('/job/status/2'),
        api.get('/job/status/3')
      ]);

      const allJobs = allJobsResponse.data;
      const myJobs = allJobs.filter(job => job.createdByUserId === user.id);

      setStats({
        totalJobs: allJobs.length,
        openJobs: openJobsResponse.data.length,
        onHoldJobs: onHoldJobsResponse.data.length,
        closedJobs: closedJobsResponse.data.length,
        myJobs: myJobs.length
      });

      // Recent jobs
      const sortedJobs = allJobs
        .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
        .slice(0, 5);
      setRecentJobs(sortedJobs);

    } catch (error) {
      console.error('Error fetching dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const getStatusBadge = (status, statusName) => {
    const statusClasses = {
      1: 'bg-green-100 text-green-800',
      2: 'bg-yellow-100 text-yellow-800',
      3: 'bg-red-100 text-red-800'
    };
    
    return (
      <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${statusClasses[status] || 'bg-gray-100 text-gray-800'}`}>
        {statusName}
      </span>
    );
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">
          Welcome back, {user.email}!
        </h1>
        <p className="mt-2 text-gray-600">
          Here's what's happening with your recruitment activities.
        </p>
      </div>

      {/* Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div className="bg-white overflow-hidden shadow rounded-lg">
          <div className="p-5">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <svg className="h-6 w-6 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2-2v2m8 0H8m8 0v2a2 2 0 01-2 2H10a2 2 0 01-2-2V6" />
                </svg>
              </div>
              <div className="ml-5 w-0 flex-1">
                <dl>
                  <dt className="text-sm font-medium text-gray-500 truncate">
                    {isAdmin() ? 'Total Jobs' : 'My Jobs'}
                  </dt>
                  <dd className="text-lg font-medium text-gray-900">
                    {isAdmin() ? stats.totalJobs : stats.myJobs}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div className="bg-white overflow-hidden shadow rounded-lg">
          <div className="p-5">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <svg className="h-6 w-6 text-green-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <div className="ml-5 w-0 flex-1">
                <dl>
                  <dt className="text-sm font-medium text-gray-500 truncate">Open Jobs</dt>
                  <dd className="text-lg font-medium text-gray-900">{stats.openJobs}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div className="bg-white overflow-hidden shadow rounded-lg">
          <div className="p-5">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <svg className="h-6 w-6 text-yellow-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <div className="ml-5 w-0 flex-1">
                <dl>
                  <dt className="text-sm font-medium text-gray-500 truncate">On Hold</dt>
                  <dd className="text-lg font-medium text-gray-900">{stats.onHoldJobs}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div className="bg-white overflow-hidden shadow rounded-lg">
          <div className="p-5">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <svg className="h-6 w-6 text-red-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
              </div>
              <div className="ml-5 w-0 flex-1">
                <dl>
                  <dt className="text-sm font-medium text-gray-500 truncate">Closed Jobs</dt>
                  <dd className="text-lg font-medium text-gray-900">{stats.closedJobs}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="bg-white shadow rounded-lg">
        <div className="px-6 py-4 border-b border-gray-200">
          <h3 className="text-lg font-medium text-gray-900">Quick Actions</h3>
        </div>
        <div className="p-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <Link
              to="/jobs/create"
              className="flex items-center p-4 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors"
            >
              <div className="flex-shrink-0">
                <svg className="h-8 w-8 text-indigo-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                </svg>
              </div>
              <div className="ml-4">
                <h4 className="text-sm font-medium text-gray-900">Create New Job</h4>
                <p className="text-sm text-gray-500">Post a new job opening</p>
              </div>
            </Link>


          </div>
        </div>
      </div>

      <div className="bg-white shadow rounded-lg">
        <div className="px-6 py-4 border-b border-gray-200">
          <div className="flex justify-between items-center">
            <h3 className="text-lg font-medium text-gray-900">Recent Job Openings</h3>
          </div>
        </div>
        <div className="divide-y divide-gray-200">
          {recentJobs.map((job) => (
            <div key={job.id} className="p-6 hover:bg-gray-50">
              <div className="flex items-center justify-between">
                <div className="flex-1">
                  <div className="flex items-center justify-between">
                    <span className="text-lg font-medium text-gray-900">
                      {job.title}
                    </span>
                    {getStatusBadge(job.status, job.statusName)}
                  </div>
                  <p className="mt-1 text-sm text-gray-600">{job.department} • {job.location}</p>
                  <p className="mt-2 text-sm text-gray-500 line-clamp-2">{job.description}</p>
                </div>
                <div className="ml-4 text-right">
                  <p className="text-sm text-gray-500">Created by</p>
                  <p className="text-sm font-medium text-gray-900">{job.createdByUserEmail}</p>
                  <p className="text-sm text-gray-500">{formatDate(job.createdAt)}</p>
                </div>
              </div>
            </div>
          ))}
        </div>
        {recentJobs.length === 0 && (
          <div className="p-6 text-center text-gray-500">
            No recent job openings found.
          </div>
        )}
      </div>
    </div>
  );
};

export default Dashboard;