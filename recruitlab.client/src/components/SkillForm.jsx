import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import api from '../services/api';

const SkillForm = ({ isEdit = false, onClose, onSuccess }) => {
  const navigate = useNavigate();
  const { id } = useParams();
  
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    skillCategoryId: ''
  });
  
  const [skillCategories, setSkillCategories] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchSkillCategories();
    if (isEdit && id) {
      fetchSkill();
    }
  }, [isEdit, id]);

  const fetchSkillCategories = async () => {
    try {
      const response = await api.get('/skill/categories');
      setSkillCategories(response.data);
    } catch (error) {
      console.error('Error fetching skill categories:', error);
    }
  };

  const fetchSkill = async () => {
    try {
      const response = await api.get(`/skill/${id}`);
      const skill = response.data;
      setFormData({
        name: skill.name,
        description: skill.description,
        skillCategoryId: skill.skillCategoryId
      });
    } catch (error) {
      setError('Failed to fetch skill details');
      console.error('Error fetching skill:', error);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const payload = {
        ...formData,
        skillCategoryId: parseInt(formData.skillCategoryId)
      };

      if (isEdit) {
        await api.put(`/skill/${id}`, payload);
      } else {
        await api.post('/skill', payload);
      }
      
      if (onSuccess) {
        onSuccess();
      } else {
        navigate('/skills');
      }
    } catch (error) {
      setError(error.response?.data?.message || 'Failed to save skill');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      {!onClose && (
        <h1 className="text-3xl font-bold text-gray-900 mb-6">
          {isEdit ? 'Edit Skill' : 'Create Skill'}
        </h1>
      )}

      <form onSubmit={handleSubmit} className="space-y-6">
        <div className="bg-white shadow rounded-lg p-6">
          <div className="space-y-6">
            <div>
              <label htmlFor="name" className="block text-sm font-medium text-gray-700">
                Skill Name *
              </label>
              <input
                type="text"
                id="name"
                name="name"
                required
                value={formData.name}
                onChange={handleChange}
                className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                placeholder="e.g., JavaScript, Project Management"
              />
            </div>

            <div>
              <label htmlFor="skillCategoryId" className="block text-sm font-medium text-gray-700">
                Category *
              </label>
              <select
                id="skillCategoryId"
                name="skillCategoryId"
                required
                value={formData.skillCategoryId}
                onChange={handleChange}
                className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              >
                <option value="">Select a category</option>
                {skillCategories.map(category => (
                  <option key={category.id} value={category.id}>
                    {category.name}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label htmlFor="description" className="block text-sm font-medium text-gray-700">
                Description
              </label>
              <textarea
                id="description"
                name="description"
                rows={4}
                value={formData.description}
                onChange={handleChange}
                className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                placeholder="Describe what this skill involves..."
              />
            </div>
          </div>
        </div>

        {error && (
          <div className="rounded-md bg-red-50 p-4">
            <div className="text-sm text-red-700">{error}</div>
          </div>
        )}

        <div className="flex justify-end space-x-4">
          <button
            type="button"
            onClick={onClose || (() => navigate('/skills'))}
            className="bg-gray-300 hover:bg-gray-400 text-gray-700 px-4 py-2 rounded-md text-sm font-medium"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={loading}
            className="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-md text-sm font-medium disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? 'Saving...' : (isEdit ? 'Update Skill' : 'Create Skill')}
          </button>
        </div>
      </form>
    </div>
  );
};

export default SkillForm;