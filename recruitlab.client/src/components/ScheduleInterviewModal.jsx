import React, { useState, useEffect } from "react";
import interviewService from "../services/interviewService";
import userService from "../services/userService";
import {
  X,
  Calendar,
  Clock,
  Loader2,
  Link as LinkIcon,
  Type,
  Users,
  CheckSquare,
  Square,
} from "lucide-react";

const ScheduleInterviewModal = ({
  applicationId,
  candidateName,
  onClose,
  onSuccess,
}) => {
  const [loading, setLoading] = useState(false);

  // 1. Array for Multiple Selection
  const [interviewersList, setInterviewersList] = useState([]);
  const [selectedInterviewerIds, setSelectedInterviewerIds] = useState([]);

  const [formData, setFormData] = useState({
    date: "",
    time: "",
    roundName: "",
    roundType: "1",
    meetLink: "",
  });

  useEffect(() => {
    const fetchInterviewers = async () => {
      try {
        const data = await userService.getInterviewers();
        setInterviewersList(data || []);
      } catch (error) {
        console.error("Failed to load interviewers", error);
      }
    };
    fetchInterviewers();
  }, []);

  // 2. Toggle Selection Helper
  const toggleInterviewer = (id) => {
    setSelectedInterviewerIds((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (selectedInterviewerIds.length === 0) {
      alert("Please assign at least one interviewer.");
      return;
    }

    setLoading(true);

    try {
      const scheduledTime = new Date(
        `${formData.date}T${formData.time}`
      ).toISOString();

      const payload = {
        applicationId: applicationId,
        roundName: formData.roundName,
        roundType: parseInt(formData.roundType),
        scheduledTime: scheduledTime,
        meetLink: formData.meetLink,

        interviewerIds: selectedInterviewerIds,
      };

      await interviewService.scheduleInterview(payload);

      alert("Interview Scheduled Successfully!");
      if (onSuccess) onSuccess();
      onClose();
    } catch (error) {
      console.error("Scheduling failed", error);
      alert("Failed to schedule interview. Check console.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
      <div className="bg-white rounded-xl shadow-xl w-full max-w-md overflow-hidden animate-in fade-in zoom-in-95 duration-200">
        {/* Header */}
        <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-gray-50">
          <h3 className="font-bold text-gray-900">Schedule Interview</h3>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 p-1 rounded-full hover:bg-gray-200 transition"
          >
            <X size={20} />
          </button>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          <div className="p-3 bg-blue-50 rounded-lg border border-blue-100 text-sm text-blue-800 mb-2">
            Candidate: <span className="font-bold">{candidateName}</span>
          </div>

          {/* 4. Multi-Select Interviewer List */}
          <div>
            <div className="flex justify-between items-end mb-1">
              <label className="text-xs font-bold text-gray-500 uppercase">
                Assign Interviewers
              </label>
              <span className="text-xs text-blue-600 font-medium">
                {selectedInterviewerIds.length} selected
              </span>
            </div>

            <div className="border border-gray-300 rounded-lg max-h-32 overflow-y-auto bg-gray-50 p-1">
              {interviewersList.length === 0 ? (
                <p className="text-xs text-orange-500 p-2">
                  No interviewers found.
                </p>
              ) : (
                interviewersList.map((user) => {
                  const isSelected = selectedInterviewerIds.includes(user.id);
                  return (
                    <div
                      key={user.id}
                      onClick={() => toggleInterviewer(user.id)}
                      className={`flex items-center gap-3 p-2 rounded cursor-pointer transition-colors ${
                        isSelected
                          ? "bg-blue-50 border border-blue-100"
                          : "hover:bg-white"
                      }`}
                    >
                      <div
                        className={`text-blue-600 ${
                          isSelected ? "opacity-100" : "opacity-40"
                        }`}
                      >
                        {isSelected ? (
                          <CheckSquare size={16} />
                        ) : (
                          <Square size={16} />
                        )}
                      </div>
                      <div>
                        <p
                          className={`text-sm font-medium ${
                            isSelected ? "text-blue-900" : "text-gray-700"
                          }`}
                        >
                          {user.name}
                        </p>
                        <p className="text-xs text-gray-400">{user.role}</p>
                      </div>
                    </div>
                  );
                })
              )}
            </div>
          </div>

          {/* Date & Time */}
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-bold text-gray-500 uppercase mb-1">
                Date
              </label>
              <div className="relative">
                <Calendar
                  className="absolute left-3 top-2.5 text-gray-400"
                  size={16}
                />
                <input
                  type="date"
                  required
                  className="w-full pl-9 pr-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-blue-500 focus:border-blue-500"
                  onChange={(e) =>
                    setFormData({ ...formData, date: e.target.value })
                  }
                />
              </div>
            </div>
            <div>
              <label className="block text-xs font-bold text-gray-500 uppercase mb-1">
                Time
              </label>
              <div className="relative">
                <Clock
                  className="absolute left-3 top-2.5 text-gray-400"
                  size={16}
                />
                <input
                  type="time"
                  required
                  className="w-full pl-9 pr-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-blue-500 focus:border-blue-500"
                  onChange={(e) =>
                    setFormData({ ...formData, time: e.target.value })
                  }
                />
              </div>
            </div>
          </div>

          {/* Round Details */}
          <div>
            <label className="block text-xs font-bold text-gray-500 uppercase mb-1">
              Round Name
            </label>
            <input
              type="text"
              required
              placeholder="e.g. Technical Round 1"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-blue-500 focus:border-blue-500"
              onChange={(e) =>
                setFormData({ ...formData, roundName: e.target.value })
              }
            />
          </div>

          <div>
            <label className="block text-xs font-bold text-gray-500 uppercase mb-1">
              Round Type
            </label>
            <div className="relative">
              <Type
                className="absolute left-3 top-2.5 text-gray-400"
                size={16}
              />
              <select
                className="w-full pl-9 pr-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-blue-500 focus:border-blue-500 bg-white"
                onChange={(e) =>
                  setFormData({ ...formData, roundType: e.target.value })
                }
                value={formData.roundType}
              >
                <option value="1">Technical</option>
                <option value="2">HR</option>
                <option value="3">Screening</option>
              </select>
            </div>
          </div>

          {/* Meet Link */}
          <div>
            <label className="block text-xs font-bold text-gray-500 uppercase mb-1">
              Meeting Link
            </label>
            <div className="relative">
              <LinkIcon
                className="absolute left-3 top-2.5 text-gray-400"
                size={16}
              />
              <input
                type="url"
                placeholder="https://meet.google.com/..."
                className="w-full pl-9 pr-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-blue-500 focus:border-blue-500"
                onChange={(e) =>
                  setFormData({ ...formData, meetLink: e.target.value })
                }
              />
            </div>
          </div>

          <div className="pt-4">
            <button
              type="submit"
              disabled={loading}
              className="w-full py-2.5 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-lg shadow-md flex justify-center items-center gap-2 transition-all active:scale-95"
            >
              {loading ? (
                <Loader2 className="animate-spin" size={18} />
              ) : (
                "Confirm Schedule"
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ScheduleInterviewModal;
