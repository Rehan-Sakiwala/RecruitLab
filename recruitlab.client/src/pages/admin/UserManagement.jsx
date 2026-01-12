import React, { useEffect, useState } from "react";
import userService from "../../services/userService";
import {
  Search,
  Mail,
  Loader2,
  ShieldCheck,
  Users,
  PieChart,
} from "lucide-react";

const UserManagement = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");

  useEffect(() => {
    fetchStaff();
  }, []);

  const fetchStaff = async () => {
    try {
      const data = await userService.getStaff();
      setUsers(Array.isArray(data) ? data : []);
    } catch (error) {
      console.error("Failed to load staff", error);
    } finally {
      setLoading(false);
    }
  };

  const getRoleStats = () => {
    const stats = { Admin: 0, Recruiter: 0, Interviewer: 0, HR: 0 };
    users.forEach((u) => {
      const role = u.role || "Unknown";
      if (stats[role] !== undefined) stats[role]++;
      else stats[role] = (stats[role] || 0) + 1;
    });
    return stats;
  };

  const roleStats = getRoleStats();
  const totalUsers = users.length;

  const getPercentage = (count) =>
    totalUsers === 0 ? 0 : Math.round((count / totalUsers) * 100);

  const getRoleBadge = (role) => {
    const colors = {
      Admin: "bg-red-100 text-red-800",
      Recruiter: "bg-blue-100 text-blue-800",
      Interviewer: "bg-purple-100 text-purple-800",
      HR: "bg-pink-100 text-pink-800",
    };
    return (
      <span
        className={`px-2 py-1 rounded-full text-xs font-bold ${
          colors[role] || "bg-gray-100"
        }`}
      >
        {role}
      </span>
    );
  };

  const filteredUsers = users.filter(
    (u) =>
      u.name?.toLowerCase().includes(search.toLowerCase()) ||
      u.email?.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="max-w-6xl mx-auto space-y-6">
      <div className="flex justify-between items-end">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Staff Directory</h1>
          <p className="text-gray-500">
            View all system administrators and staff members.
          </p>
        </div>

        <div className="relative w-64">
          <Search className="absolute left-3 top-2.5 text-gray-400" size={16} />
          <input
            type="text"
            placeholder="Search staff..."
            className="pl-9 pr-4 py-2 border border-gray-300 rounded-lg text-sm w-full focus:ring-blue-500 focus:border-blue-500"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="bg-white p-5 rounded-xl border border-gray-200 shadow-sm flex items-center gap-4">
          <div className="p-3 bg-blue-50 text-blue-600 rounded-lg">
            <Users size={24} />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-500">Total Staff</p>
            <h4 className="text-2xl font-bold text-gray-900">{totalUsers}</h4>
          </div>
        </div>

        <div className="md:col-span-3 bg-white p-5 rounded-xl border border-gray-200 shadow-sm flex items-center justify-around">
          {Object.entries(roleStats).map(([role, count]) => (
            <div key={role} className="text-center">
              <div className="text-sm font-medium text-gray-500 mb-1">
                {role}
              </div>
              <div className="flex items-center justify-center gap-2">
                <span className="text-xl font-bold text-gray-900">{count}</span>
                <span
                  className={`text-xs px-1.5 py-0.5 rounded ${
                    role === "Admin"
                      ? "bg-red-50 text-red-600"
                      : role === "Recruiter"
                      ? "bg-blue-50 text-blue-600"
                      : role === "Interviewer"
                      ? "bg-purple-50 text-purple-600"
                      : "bg-pink-50 text-pink-600"
                  }`}
                >
                  {getPercentage(count)}%
                </span>
              </div>
              <div className="w-16 h-1 bg-gray-100 rounded-full mt-2 mx-auto overflow-hidden">
                <div
                  className={`h-full rounded-full ${
                    role === "Admin"
                      ? "bg-red-500"
                      : role === "Recruiter"
                      ? "bg-blue-500"
                      : role === "Interviewer"
                      ? "bg-purple-500"
                      : "bg-pink-500"
                  }`}
                  style={{ width: `${getPercentage(count)}%` }}
                ></div>
              </div>
            </div>
          ))}
        </div>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
        {loading ? (
          <div className="p-12 flex justify-center">
            <Loader2 className="animate-spin text-blue-600" />
          </div>
        ) : filteredUsers.length === 0 ? (
          <div className="p-10 text-center text-gray-500">No users found.</div>
        ) : (
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  User
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Role
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Status
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  Details
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {filteredUsers.map((u) => (
                <tr key={u.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4">
                    <div className="flex items-center">
                      <div className="h-10 w-10 rounded-full bg-gradient-to-br from-gray-100 to-gray-200 flex items-center justify-center text-gray-600 font-bold mr-3 border border-gray-200">
                        {u.name?.[0] || "U"}
                      </div>
                      <div>
                        <div className="text-sm font-medium text-gray-900">
                          {u.name}
                        </div>
                        <div className="text-xs text-gray-500 flex items-center gap-1">
                          <Mail size={10} /> {u.email}
                        </div>
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4">{getRoleBadge(u.role)}</td>
                  <td className="px-6 py-4">
                    <span className="flex items-center gap-1.5 text-green-700 bg-green-50 px-2 py-1 rounded text-xs font-medium w-fit border border-green-100">
                      <ShieldCheck size={12} /> Active
                    </span>
                  </td>
                  <td className="px-6 py-4 text-right text-sm">
                    <button className="text-gray-400 hover:text-blue-600 transition-colors font-medium text-xs">
                      View Profile
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
};

export default UserManagement;
