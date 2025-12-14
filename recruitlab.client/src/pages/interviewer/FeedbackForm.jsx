import React, { useEffect, useState } from "react";
import { useParams, useLocation, useNavigate } from "react-router-dom";
import interviewService from "../../services/interviewService";
import jobService from "../../services/jobService";
import { Star, ArrowLeft, Plus, Trash2, CheckCircle } from "lucide-react";

const FeedbackForm = () => {
  const { interviewId } = useParams();
  const navigate = useNavigate();
  const location = useLocation();
  const taskInfo = location.state?.task;

  const [loading, setLoading] = useState(false);
  const [masterSkills, setMasterSkills] = useState([]);

  // Form State
  const [overallRating, setOverallRating] = useState(3);
  const [comments, setComments] = useState("");
  const [skillRatings, setSkillRatings] = useState([]);

  useEffect(() => {
    const loadSkills = async () => {
      const res = await jobService.getAllSkills();
      setMasterSkills(res.data);
    };
    loadSkills();
  }, []);

  const handleAddSkill = () => {
    setSkillRatings([
      ...skillRatings,
      { skillId: "", rating: 3, comments: "" },
    ]);
  };

  const updateSkillRating = (index, field, value) => {
    const updated = [...skillRatings];
    updated[index][field] = value;
    setSkillRatings(updated);
  };

  const removeSkillRating = (index) => {
    setSkillRatings(skillRatings.filter((_, i) => i !== index));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!comments.trim()) return alert("Please add overall comments.");

    try {
      setLoading(true);
      const payload = {
        interviewId: parseInt(interviewId),
        overallRating: parseInt(overallRating),
        comments: comments,
        skillRatings: skillRatings
          .filter((s) => s.skillId)
          .map((s) => ({
            skillId: parseInt(s.skillId),
            rating: parseInt(s.rating),
            comments: s.comments,
          })),
      };

      await interviewService.submitFeedback(payload);
      alert("Feedback submitted successfully!");
      navigate("/interviewer/dashboard");
    } catch (err) {
      alert(
        "Submission failed: " + (err.response?.data?.message || err.message)
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-3xl mx-auto">
      <button
        onClick={() => navigate(-1)}
        className="flex items-center text-gray-500 hover:text-gray-900 mb-6 transition-colors"
      >
        <ArrowLeft size={18} className="mr-1" /> Back to Dashboard
      </button>

      <div className="bg-white rounded-xl border border-gray-200 shadow-sm overflow-hidden">
        {/* Header */}
        <div className="bg-gray-50 px-6 py-4 border-b border-gray-200">
          <h1 className="text-xl font-bold text-gray-900">
            Interview Feedback
          </h1>
          {taskInfo && (
            <p className="text-sm text-gray-500 mt-1">
              Candidate:{" "}
              <span className="font-medium text-gray-800">
                {taskInfo.candidateName}
              </span>{" "}
              • Role:{" "}
              <span className="font-medium text-gray-800">
                {taskInfo.jobTitle}
              </span>
            </p>
          )}
        </div>

        <form onSubmit={handleSubmit} className="p-6 space-y-8">
          {/* 1. Overall Rating */}
          <div>
            <label className="block text-sm font-bold text-gray-700 mb-3 uppercase tracking-wide">
              Overall Rating
            </label>
            <div className="flex gap-4">
              {[1, 2, 3, 4, 5].map((star) => (
                <button
                  key={star}
                  type="button"
                  onClick={() => setOverallRating(star)}
                  className={`flex flex-col items-center gap-1 transition-transform hover:scale-110 focus:outline-none ${
                    overallRating >= star ? "text-yellow-400" : "text-gray-300"
                  }`}
                >
                  <Star
                    size={32}
                    fill={overallRating >= star ? "currentColor" : "none"}
                  />
                  <span className="text-xs font-medium text-gray-500">
                    {star}
                  </span>
                </button>
              ))}
            </div>
          </div>

          {/* 2. Detailed Comments */}
          <div>
            <label className="block text-sm font-bold text-gray-700 mb-2 uppercase tracking-wide">
              Detailed Evaluation
            </label>
            <textarea
              className="w-full border border-gray-300 rounded-lg p-3 h-32 focus:ring-2 focus:ring-purple-500 focus:outline-none"
              placeholder="Describe the candidate's performance, strengths, and weaknesses..."
              value={comments}
              onChange={(e) => setComments(e.target.value)}
              required
            ></textarea>
          </div>

          <hr className="border-gray-100" />

          {/* 3. Skill Specific Ratings */}
          <div>
            <div className="flex justify-between items-center mb-4">
              <label className="block text-sm font-bold text-gray-700 uppercase tracking-wide">
                Technical Skill Ratings (Optional)
              </label>
              <button
                type="button"
                onClick={handleAddSkill}
                className="text-sm flex items-center gap-1 text-purple-600 hover:text-purple-700 font-medium"
              >
                <Plus size={16} /> Add Skill
              </button>
            </div>

            <div className="space-y-3">
              {skillRatings.map((skill, index) => (
                <div
                  key={index}
                  className="flex gap-3 items-start bg-gray-50 p-3 rounded-lg border border-gray-200"
                >
                  <div className="flex-1">
                    <select
                      className="w-full border border-gray-300 rounded p-2 text-sm mb-2"
                      value={skill.skillId}
                      onChange={(e) =>
                        updateSkillRating(index, "skillId", e.target.value)
                      }
                    >
                      <option value="">Select Skill...</option>
                      {masterSkills.map((s) => (
                        <option key={s.id} value={s.id}>
                          {s.name}
                        </option>
                      ))}
                    </select>
                    <input
                      type="text"
                      placeholder="Specific comment (optional)"
                      className="w-full border border-gray-300 rounded p-2 text-sm"
                      value={skill.comments}
                      onChange={(e) =>
                        updateSkillRating(index, "comments", e.target.value)
                      }
                    />
                  </div>

                  <div className="w-24">
                    <select
                      className="w-full border border-gray-300 rounded p-2 text-sm font-medium"
                      value={skill.rating}
                      onChange={(e) =>
                        updateSkillRating(index, "rating", e.target.value)
                      }
                    >
                      <option value="1">1 - Poor</option>
                      <option value="2">2 - Fair</option>
                      <option value="3">3 - Good</option>
                      <option value="4">4 - V. Good</option>
                      <option value="5">5 - Excellent</option>
                    </select>
                  </div>

                  <button
                    type="button"
                    onClick={() => removeSkillRating(index)}
                    className="p-2 text-gray-400 hover:text-red-500"
                  >
                    <Trash2 size={18} />
                  </button>
                </div>
              ))}

              {skillRatings.length === 0 && (
                <p className="text-sm text-gray-400 italic">
                  No specific skills added. Click "Add Skill" to rate specific
                  technologies.
                </p>
              )}
            </div>
          </div>

          <div className="pt-4">
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-purple-600 text-white font-bold py-3 px-4 rounded-lg hover:bg-purple-700 transition-colors shadow-sm disabled:opacity-50 flex justify-center items-center gap-2"
            >
              {loading ? (
                "Submitting..."
              ) : (
                <>
                  <CheckCircle size={20} /> Submit Feedback
                </>
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default FeedbackForm;
