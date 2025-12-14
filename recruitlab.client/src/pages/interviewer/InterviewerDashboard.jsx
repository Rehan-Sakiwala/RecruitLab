import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import interviewService from "../../services/interviewService";
import {
  Calendar,
  Clock,
  Video,
  FileText,
  User,
  Briefcase,
  Loader2,
} from "lucide-react";

const InterviewerDashboard = () => {
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    fetchTasks();
  }, []);

  const fetchTasks = async () => {
    try {
      const res = await interviewService.getMyInterviews();
      setTasks(res.data);
    } catch (err) {
      console.error("Failed to load tasks", err);
    } finally {
      setLoading(false);
    }
  };

  if (loading)
    return (
      <div className="flex justify-center p-20">
        <Loader2 className="animate-spin text-purple-600" />
      </div>
    );

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">My Interview Tasks</h1>
        <p className="text-gray-500">Upcoming interviews assigned to you.</p>
      </div>

      {tasks.length === 0 ? (
        <div className="bg-white rounded-xl border border-gray-200 p-12 text-center">
          <div className="mx-auto w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mb-4">
            <Calendar className="text-gray-400" size={32} />
          </div>
          <h3 className="text-lg font-medium text-gray-900">
            No Pending Interviews
          </h3>
          <p className="text-gray-500 mt-1">
            You're all caught up! Check back later for new assignments.
          </p>
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-6">
          {tasks.map((task) => (
            <div
              key={task.interviewId}
              className="bg-white rounded-xl border border-gray-200 shadow-sm hover:shadow-md transition-shadow p-6 flex flex-col md:flex-row justify-between gap-6"
            >
              {/* Left Info */}
              <div className="flex-1 space-y-4">
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <span
                      className={`px-2.5 py-0.5 rounded-full text-xs font-semibold ${
                        task.type === "Technical"
                          ? "bg-blue-100 text-blue-800"
                          : "bg-purple-100 text-purple-800"
                      }`}
                    >
                      {task.type}
                    </span>
                    <span className="text-xs text-gray-400 font-medium uppercase tracking-wider">
                      Round: {task.round}
                    </span>
                  </div>
                  <h3 className="text-xl font-bold text-gray-900">
                    {task.candidateName}
                  </h3>
                  <div className="flex items-center gap-2 text-gray-600 mt-1">
                    <Briefcase size={16} />
                    <span>{task.jobTitle}</span>
                  </div>
                </div>

                <div className="flex flex-wrap gap-6 text-sm text-gray-600">
                  <div className="flex items-center gap-2">
                    <Calendar size={18} className="text-gray-400" />
                    <span>
                      {new Date(task.date).toLocaleDateString(undefined, {
                        weekday: "long",
                        year: "numeric",
                        month: "long",
                        day: "numeric",
                      })}
                    </span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Clock size={18} className="text-gray-400" />
                    <span>
                      {new Date(task.date).toLocaleTimeString([], {
                        hour: "2-digit",
                        minute: "2-digit",
                      })}
                    </span>
                  </div>
                </div>
              </div>

              {/* Right Actions */}
              <div className="flex flex-col gap-3 justify-center min-w-[200px]">
                {task.meetLink && (
                  <a
                    href={
                      task.meetLink.startsWith("http")
                        ? task.meetLink
                        : `https://${task.meetLink}`
                    }
                    target="_blank"
                    rel="noopener noreferrer"
                    className="flex items-center justify-center gap-2 bg-blue-600 text-white px-4 py-2.5 rounded-lg hover:bg-blue-700 transition-colors font-medium"
                  >
                    <Video size={18} />
                    Join Meeting
                  </a>
                )}

                <button
                  onClick={() =>
                    navigate(`/interviewer/feedback/${task.interviewId}`, {
                      state: { task },
                    })
                  }
                  className="flex items-center justify-center gap-2 bg-white border border-gray-300 text-gray-700 px-4 py-2.5 rounded-lg hover:bg-gray-50 transition-colors font-medium"
                >
                  <FileText size={18} />
                  Submit Feedback
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default InterviewerDashboard;
