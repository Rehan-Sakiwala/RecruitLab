import React, { useState, useEffect } from "react";
import jobService from "../../services/jobService";
import { Plus, Trash2, Save, X, AlertCircle } from "lucide-react";

const JobForm = ({ initialData = null, onSubmit, isEdit = false }) => {
  // Form State
  const [formData, setFormData] = useState({
    title: "",
    department: "",
    location: "",
    description: "",
    salaryMin: "",
    salaryMax: "",
    status: 1, // Default Open
    statusReason: "",
    jobSkills: [],
  });

  // Skills Management State
  const [availableSkills, setAvailableSkills] = useState([]);
  const [skillCategories, setSkillCategories] = useState([]); // Store categories

  // New Skill UI State
  const [isAddingNewSkill, setIsAddingNewSkill] = useState(false);
  const [newSkillName, setNewSkillName] = useState("");
  const [newSkillCategoryId, setNewSkillCategoryId] = useState(""); // Store selected category

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  // Load data on mount
  useEffect(() => {
    if (initialData) {
      setFormData(initialData);
    }
    loadData();
  }, [initialData]);

  const loadData = async () => {
    try {
      const [skillsRes, categoriesRes] = await Promise.all([
        jobService.getAllSkills(),
        jobService.getAllSkillCategories(),
      ]);
      setAvailableSkills(skillsRes.data);
      setSkillCategories(categoriesRes.data);

      // Set default category if available
      if (categoriesRes.data.length > 0) {
        setNewSkillCategoryId(categoriesRes.data[0].id);
      }
    } catch (err) {
      console.error("Failed to load skills or categories", err);
    }
  };

  // --- Handlers ---

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSkillChange = (index, field, value) => {
    const updatedSkills = [...formData.jobSkills];
    updatedSkills[index] = { ...updatedSkills[index], [field]: value };
    setFormData((prev) => ({ ...prev, jobSkills: updatedSkills }));
  };

  const addSkillRow = () => {
    setFormData((prev) => ({
      ...prev,
      jobSkills: [
        ...prev.jobSkills,
        { skillId: "", requirementType: 1, yearsOfExperience: 0, notes: "" },
      ],
    }));
  };

  const removeSkillRow = (index) => {
    const updated = formData.jobSkills.filter((_, i) => i !== index);
    setFormData((prev) => ({ ...prev, jobSkills: updated }));
  };

  // Create New Master Skill
  const handleCreateNewSkill = async () => {
    if (!newSkillName.trim() || !newSkillCategoryId) {
      alert("Please enter a name and select a category.");
      return;
    }

    try {
      const newSkillDto = {
        name: newSkillName,
        description: newSkillName,
        skillCategoryId: parseInt(newSkillCategoryId), // Use selected category
      };

      const res = await jobService.createSkill(newSkillDto);

      // Update UI
      setAvailableSkills([...availableSkills, res.data]);
      setNewSkillName("");
      setIsAddingNewSkill(false);
    } catch (err) {
      alert(
        "Failed to create skill. " +
          (err.response?.data?.message || err.message)
      );
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    const validSkills = formData.jobSkills.filter((s) => s.skillId);
    const payload = { ...formData, jobSkills: validSkills };

    onSubmit(payload).catch((err) => {
      setError(err.response?.data?.message || err.message);
      setLoading(false);
    });
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 max-w-4xl mx-auto"
    >
      {error && (
        <div className="mb-4 p-4 bg-red-50 text-red-700 rounded-lg flex items-center gap-2">
          <AlertCircle size={20} />
          {error}
        </div>
      )}

      <div className="space-y-8">
        {/* Section 1: Job Details */}
        <div>
          <h3 className="text-lg font-bold text-gray-900 mb-4 pb-2 border-b border-gray-100">
            Job Details
          </h3>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Job Title *
              </label>
              <input
                type="text"
                name="title"
                required
                value={formData.title}
                onChange={handleChange}
                className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Department *
              </label>
              <input
                type="text"
                name="department"
                required
                value={formData.department}
                onChange={handleChange}
                className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Location *
              </label>
              <input
                type="text"
                name="location"
                required
                value={formData.location}
                onChange={handleChange}
                className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>
            {isEdit && (
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Status
                </label>
                <select
                  name="status"
                  value={formData.status}
                  onChange={handleChange}
                  className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                >
                  <option value={1}>Open</option>
                  <option value={2}>On Hold</option>
                  <option value={3}>Closed</option>
                </select>
              </div>
            )}
          </div>

          <div className="mt-5 grid grid-cols-2 gap-5">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Min Salary
              </label>
              <input
                type="number"
                name="salaryMin"
                value={formData.salaryMin}
                onChange={handleChange}
                className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Max Salary
              </label>
              <input
                type="number"
                name="salaryMax"
                value={formData.salaryMax}
                onChange={handleChange}
                className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>
          </div>

          <div className="mt-5">
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Description *
            </label>
            <textarea
              name="description"
              rows={5}
              required
              value={formData.description}
              onChange={handleChange}
              className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            />
          </div>
        </div>

        {/* Section 2: Skills Requirements */}
        <div>
          <div className="flex justify-between items-center mb-4 pb-2 border-b border-gray-100">
            <h3 className="text-lg font-bold text-gray-900">Required Skills</h3>

            {/* Quick Add Skill Form (UPDATED) */}
            <div className="flex items-center gap-2">
              {isAddingNewSkill ? (
                <div className="flex items-center gap-2 bg-gray-50 p-1.5 rounded-lg border border-gray-200">
                  <input
                    type="text"
                    placeholder="Skill Name"
                    className="border-gray-300 rounded px-2 py-1 text-sm w-32 focus:ring-blue-500 focus:border-blue-500"
                    value={newSkillName}
                    onChange={(e) => setNewSkillName(e.target.value)}
                  />

                  {/* Category Dropdown */}
                  <select
                    value={newSkillCategoryId}
                    onChange={(e) => setNewSkillCategoryId(e.target.value)}
                    className="border-gray-300 rounded px-2 py-1 text-sm w-32 focus:ring-blue-500 focus:border-blue-500"
                  >
                    <option value="">Category</option>
                    {skillCategories.map((cat) => (
                      <option key={cat.id} value={cat.id}>
                        {cat.name}
                      </option>
                    ))}
                  </select>

                  <button
                    type="button"
                    onClick={handleCreateNewSkill}
                    className="text-green-600 hover:bg-green-100 p-1 rounded"
                    title="Save"
                  >
                    <Save size={16} />
                  </button>
                  <button
                    type="button"
                    onClick={() => setIsAddingNewSkill(false)}
                    className="text-red-600 hover:bg-red-100 p-1 rounded"
                    title="Cancel"
                  >
                    <X size={16} />
                  </button>
                </div>
              ) : (
                <button
                  type="button"
                  onClick={() => setIsAddingNewSkill(true)}
                  className="text-xs text-blue-600 hover:text-blue-800 font-medium"
                >
                  + Create New Master Skill
                </button>
              )}
            </div>
          </div>

          <div className="space-y-3 bg-gray-50 p-4 rounded-xl border border-gray-100">
            {formData.jobSkills.length === 0 && (
              <p className="text-sm text-gray-500 text-center py-2">
                No skills added yet.
              </p>
            )}

            {formData.jobSkills.map((js, index) => (
              <div
                key={index}
                className="flex flex-col sm:flex-row gap-3 items-start sm:items-end bg-white p-3 rounded-lg border border-gray-200 shadow-sm"
              >
                <div className="flex-1 w-full">
                  <label className="text-xs text-gray-500 mb-1 block">
                    Skill
                  </label>
                  <select
                    value={js.skillId}
                    onChange={(e) =>
                      handleSkillChange(index, "skillId", e.target.value)
                    }
                    className="w-full text-sm border-gray-300 rounded-md py-1.5 focus:ring-blue-500 focus:border-blue-500"
                  >
                    <option value="">Select Skill</option>
                    {availableSkills.map((s) => (
                      <option key={s.id} value={s.id}>
                        {s.name}
                      </option>
                    ))}
                  </select>
                </div>
                <div className="w-full sm:w-32">
                  <label className="text-xs text-gray-500 mb-1 block">
                    Type
                  </label>
                  <select
                    value={js.requirementType}
                    onChange={(e) =>
                      handleSkillChange(
                        index,
                        "requirementType",
                        parseInt(e.target.value)
                      )
                    }
                    className="w-full text-sm border-gray-300 rounded-md py-1.5 focus:ring-blue-500 focus:border-blue-500"
                  >
                    <option value={1}>Required</option>
                    <option value={2}>Preferred</option>
                  </select>
                </div>
                <div className="w-full sm:w-24">
                  <label className="text-xs text-gray-500 mb-1 block">
                    Exp (Yrs)
                  </label>
                  <input
                    type="number"
                    value={js.yearsOfExperience}
                    onChange={(e) =>
                      handleSkillChange(
                        index,
                        "yearsOfExperience",
                        e.target.value
                      )
                    }
                    className="w-full text-sm border-gray-300 rounded-md py-1.5 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>
                <button
                  type="button"
                  onClick={() => removeSkillRow(index)}
                  className="text-red-500 p-2 hover:bg-red-50 rounded-lg self-end sm:self-auto"
                >
                  <Trash2 size={18} />
                </button>
              </div>
            ))}

            <button
              type="button"
              onClick={addSkillRow}
              className="mt-2 flex items-center text-sm text-blue-600 font-medium hover:text-blue-800"
            >
              <Plus size={16} className="mr-1" /> Add Skill Requirement
            </button>
          </div>
        </div>

        {/* Action Buttons */}
        <div className="flex justify-end gap-3 pt-6 border-t border-gray-100">
          <button
            type="button"
            className="px-5 py-2.5 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={loading}
            className="px-6 py-2.5 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 shadow-sm disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? "Saving..." : isEdit ? "Update Job" : "Publish Job"}
          </button>
        </div>
      </div>
    </form>
  );
};

export default JobForm;
