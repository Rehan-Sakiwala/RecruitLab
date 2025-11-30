import axios from 'axios';
import { getAuthHeaders } from '../utils/authHeaders';

const API_URL = 'https://localhost:7100/api/candidate-profile';
const CANDIDATE_API_URL = 'https://localhost:7100/api/Candidate';

const candidateProfileService = {
  // Get Full Profile
  getProfile: (id) => axios.get(`${CANDIDATE_API_URL}/${id}`, getAuthHeaders()),

  // Education
  addEducation: (data) => axios.post(`${API_URL}/education`, data, getAuthHeaders()),
  deleteEducation: (id) => axios.delete(`${API_URL}/education/${id}`, getAuthHeaders()),

  // Experience
  addExperience: (data) => axios.post(`${API_URL}/experience`, data, getAuthHeaders()),
  deleteExperience: (id) => axios.delete(`${API_URL}/experience/${id}`, getAuthHeaders()),

  // Skills
  addSkill: (data) => axios.post(`${API_URL}/skill`, data, getAuthHeaders()),
  deleteSkill: (id) => axios.delete(`${API_URL}/skill/${id}`, getAuthHeaders()),

  // CV
  uploadCv: (formData) => axios.post(`${API_URL}/cv`, formData, {
    headers: {
      ...getAuthHeaders().headers,
      'Content-Type': 'multipart/form-data',
    },
  }),
  deleteDocument: (id) => axios.delete(`${API_URL}/document/${id}`, getAuthHeaders()),
};

export default candidateProfileService;