import React from "react";
import { Outlet, Link, useNavigate, useLocation } from "react-router-dom";
import { useDispatch } from "react-redux";
import { logout, reset } from "../features/auth/authSlice";
import {
  LayoutDashboard,
  Users,
  Shield,
  LogOut,
  BarChart3,
} from "lucide-react";

const AdminLayout = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const location = useLocation();

  const handleLogout = () => {
    dispatch(logout());
    dispatch(reset());
    navigate("/login");
  };

  const navItems = [
    {
      name: "User Management",
      path: "/admin/users",
      icon: <Users size={20} />,
    },
    {
      name: "System Reports",
      path: "/admin/reports",
      icon: <BarChart3 size={20} />,
    },
  ];

  return (
    <div className="min-h-screen bg-gray-100 flex font-sans">
      <aside className="w-64 bg-gray-900 text-white fixed h-full z-10 hidden md:flex flex-col">
        <div className="h-16 flex items-center px-6 border-b border-gray-800">
          <Shield className="text-blue-400 mr-2" size={24} />
          <span className="font-bold text-xl tracking-tight">AdminPanel</span>
        </div>

        <nav className="flex-1 px-4 py-6 space-y-2">
          {navItems.map((item) => (
            <Link
              key={item.name}
              to={item.path}
              className={`flex items-center px-3 py-3 text-sm font-medium rounded-lg transition-colors ${
                location.pathname.startsWith(item.path)
                  ? "bg-blue-600 text-white"
                  : "text-gray-400 hover:bg-gray-800 hover:text-white"
              }`}
            >
              <span className="mr-3">{item.icon}</span>
              {item.name}
            </Link>
          ))}
        </nav>

        <div className="p-4 border-t border-gray-800">
          <button
            onClick={handleLogout}
            className="flex items-center w-full px-3 py-2 text-sm font-medium text-red-400 hover:bg-gray-800 rounded-lg transition-colors"
          >
            <LogOut size={20} className="mr-3" /> Logout
          </button>
        </div>
      </aside>

      <div className="flex-1 md:ml-64 p-8">
        <Outlet />
      </div>
    </div>
  );
};

export default AdminLayout;
