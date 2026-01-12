import React, { useState } from "react";
import interviewService from "../services/interviewService";
import {
  X,
  Calendar,
  Clock,
  Loader2,
  Link as LinkIcon,
  Type,
} from "lucide-react";

const ScheduleInterviewModal = ({
  applicationId,
  candidateName,
  onClose,
  onSuccess,
}) => {
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    date: "",
    time: "",
    roundName: "",
    roundType: "1",
    meetLink: "",
  });

  const handleSubmit = async (e) => {
    e.preventDefault();
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
        interviewerIds: [],
      };

      //Call API
      await interviewService.scheduleInterview(payload);

      alert("Interview Scheduled Successfully!");
      if (onSuccess) onSuccess();
      onClose();
    } catch (error) {
      console.error("Scheduling failed", error);
      alert("Failed to schedule interview. Check console details.");
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

          {/* Date & Time Row */}
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
