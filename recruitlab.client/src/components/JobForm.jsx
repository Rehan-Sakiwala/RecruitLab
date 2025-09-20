import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import api from '../services/api';

const JobForm = ({ isEdit = false }) => {
  const navigate = useNavigate();
  const { id } = useParams();
  
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    department: '',
    location: '',
    salaryMin: '',
    salaryMax: '',
    status: 1,
    statusReason: ''
  });
  
  const [skills, setSkills] = useState([]);
  const [availableSkills, setAvailableSkills] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchSkills();
    if (isEdit && id) {
      fetchJob();
    }
  }, [isEdit, id]);

  const fetchSkills = async () => {
    try {
      const response = await api.get('/skill');
      setAvailableSkills(response.data);
    } catch (error) {
      console.error('Error fetching skills:', error);
    }
  };

  const fetchJob = async () => {
    try {
      const response = await api.get(`/job/${id}`);
      const job = response.data;
      setFormData({
        title: job.title,
        description: job.description,
        department: job.department,
        location: job.location,
        salaryMin: job.salaryMin || '',
        salaryMax: job.salaryMax || '',
        status: job.status,
        statusReason: job.statusReason || ''
      });
      setSkills(job.jobSkills || []);
    } catch (error) {
      setError('Failed to fetch job details');
      console.error('Error fetching job:', error);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const addSkill = () => {
    setSkills(prev => [...prev, {
      skillId: '',
      requirementType: 1,
      yearsOfExperience: 0,
      notes: ''
    }]);
  };

  const updateSkill = (index, field, value) => {
    setSkills(prev => prev.map((skill, i) => 
      i === index ? { ...skill, [field]: value } : skill
    ));
  };

  const removeSkill = (index) => {
    setSkills(prev => prev.filter((_, i) => i !== index));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const payload = {
        ...formData,
        salaryMin: formData.salaryMin ? parseFloat(formData.salaryMin) : null,
        salaryMax: formData.salaryMax ? parseFloat(formData.salaryMax) : null,
        jobSkills: skills.filter(skill => skill.skillId)
      };

      if (isEdit) {
        await api.put(`/job/${id}`, payload);
      } else {
        await api.post('/job', payload);
      }
      
      navigate('/jobs');
    } catch (error) {
      setError(error.response?.data?.message || 'Failed to save job');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-4xl mx-auto">
      <h1 className="text-3xl font-bold text-gray-900 mb-6">
        {isEdit ? 'Edit Job Opening' : 'Create Job Opening'}
      </h1>

      <form onSubmit={handleSubmit} className="space-y-6">
        <div className="bg-white shadow rounded-lg p-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label htmlFor="title" className="block text-sm font-medium text-gray-700">
                Job Title *
              </label>
              <input
                type="text"
                id="title"
                name="title"
                required
                value={formData.title}
                onChange={handleChange}
                className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
            </div>

            <div>
              <label htmlFor="department" className="block text-sm font-medium text-gray-700">
                Department *
              </label>
              <input
                type="text"
                id="department"
                name="department"
                required
                value={formData.department}
                onChange={handleChange}
                className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
            </div>

            <div>
              <label htmlFor="location" className="block text-sm font-medium text-gray-700">
                Location *
              </label>
              <input
                type="text"
                id="location"
                name="location"
                required
                value={formData.location}
                onChange={handleChange}
                className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
            </div>

            <div>
              <label htmlFor="salaryMin" className="block text-sm font-medium text-gray-700">
                Minimum Salary
              </label>
              <input
                type="number"
                id="salaryMin"
                name="salaryMin"
                value={formData.salaryMin}
                onChange={handleChange}
                className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
            </div>

            <div>
              <label htmlFor="salaryMax" className="block text-sm font-medium text-gray-700">
                Maximum Salary
              </label>
              <input
                type="number"
                id="salaryMax"
                name="salaryMax"
                value={formData.salaryMax}
                onChange={handleChange}
                className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
            </div>

            {isEdit && (
              <div>
                <label htmlFor="status" className="block text-sm font-medium text-gray-700">
                  Status
                </label>
                <select
                  id="status"
                  name="status"
                  value={formData.status}
                  onChange={handleChange}
                  className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                >
                  <option value={1}>Open</option>
                  <option value={2}>On Hold</option>
                  <option value={3}>Closed</option>
                </select>
              </div>
            )}
          </div>

          <div className="mt-6">
            <label htmlFor="description" className="block text-sm font-medium text-gray-700">
              Description *
            </label>
            <textarea
              id="description"
              name="description"
              required
              rows={4}
              value={formData.description}
              onChange={handleChange}
              className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
            />
          </div>

          {isEdit && formData.status !== 1 && (
            <div className="mt-6">
              <label htmlFor="statusReason" className="block text-sm font-medium text-gray-700">
                Status Reason
              </label>
              <textarea
                id="statusReason"
                name="statusReason"
                rows={3}
                value={formData.statusReason}
                onChange={handleChange}
                className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
            </div>
          )}
        </div>

        <div className="bg-white shadow rounded-lg p-6">
          <div className="flex justify-between items-center mb-4">
            <h3 className="text-lg font-medium text-gray-900">Required Skills</h3>
            <button
              type="button"
              onClick={addSkill}
              className="bg-indigo-600 hover:bg-indigo-700 text-white px-3 py-2 rounded-md text-sm font-medium"
            >
              Add Skill
            </button>
          </div>

          {skills.map((skill, index) => (
            <div key={index} className="border border-gray-200 rounded-lg p-4 mb-4">
              <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700">Skill</label>
                  <select
                    value={skill.skillId}
                    onChange={(e) => updateSkill(index, 'skillId', e.target.value)}
                    className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                  >
                    <option value="">Select a skill</option>
                    {availableSkills.map(skillOption => (
                      <option key={skillOption.id} value={skillOption.id}>
                        {skillOption.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Requirement Type</label>
                  <select
                    value={skill.requirementType}
                    onChange={(e) => updateSkill(index, 'requirementType', parseInt(e.target.value))}
                    className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                  >
                    <option value={1}>Required</option>
                    <option value={2}>Preferred</option>
                    <option value={3}>Nice to Have</option>
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Years of Experience</label>
                  <input
                    type="number"
                    min="0"
                    value={skill.yearsOfExperience}
                    onChange={(e) => updateSkill(index, 'yearsOfExperience', parseInt(e.target.value))}
                    className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                  />
                </div>

                <div className="flex items-end">
                  <button
                    type="button"
                    onClick={() => removeSkill(index)}
                    className="bg-red-600 hover:bg-red-700 text-white px-3 py-2 rounded-md text-sm font-medium"
                  >
                    Remove
                  </button>
                </div>
              </div>

              <div className="mt-4">
                <label className="block text-sm font-medium text-gray-700">Notes</label>
                <input
                  type="text"
                  value={skill.notes}
                  onChange={(e) => updateSkill(index, 'notes', e.target.value)}
                  placeholder="Optional notes about this skill requirement"
                  className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                />
              </div>
            </div>
          ))}
        </div>

        {error && (
          <div className="rounded-md bg-red-50 p-4">
            <div className="text-sm text-red-700">{error}</div>
          </div>
        )}

        <div className="flex justify-end space-x-4">
          <button
            type="button"
            onClick={() => navigate('/jobs')}
            className="bg-gray-300 hover:bg-gray-400 text-gray-700 px-4 py-2 rounded-md text-sm font-medium"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={loading}
            className="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-md text-sm font-medium disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? 'Saving...' : (isEdit ? 'Update Job' : 'Create Job')}
          </button>
        </div>
      </form>
    </div>
  );
};

export default JobForm;
