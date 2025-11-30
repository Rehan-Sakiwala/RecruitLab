import axios from 'axios';
import { getAuthHeaders } from '../utils/authHeaders';

const API_URL = 'https://localhost:7100/api';

const jobService = {
  // --- JOB ENDPOINTS ---
  getAllJobs: () => axios.get(`${API_URL}/Job`, getAuthHeaders()),
  getJobById: (id) => axios.get(`${API_URL}/Job/${id}`, getAuthHeaders()),
  createJob: (data) => axios.post(`${API_URL}/Job`, data, getAuthHeaders()),
  updateJob: (id, data) => axios.put(`${API_URL}/Job/${id}`, data, getAuthHeaders()),
  deleteJob: (id) => axios.delete(`${API_URL}/Job/${id}`, getAuthHeaders()),

  // --- SKILL ENDPOINTS ---
  getAllSkills: () => axios.get(`${API_URL}/Skill`, getAuthHeaders()),

  getAllSkillCategories: () => axios.get(`${API_URL}/Skill/categories`, getAuthHeaders()),
  
  createSkill: (skillData) => axios.post(`${API_URL}/Skill`, skillData, getAuthHeaders()),
};

export default jobService;