import axios from 'axios';
import { getAuthHeaders } from '../utils/authHeaders';

const PROFILE_API_URL = 'https://localhost:7100/api/candidate-profile';
const CANDIDATE_API_URL = 'https://localhost:7100/api/Candidate';

const candidateProfileService = {
  getMyProfile: () => axios.get(`${CANDIDATE_API_URL}/my-profile`, getAuthHeaders()),
  
  updatePersonalInfo: (data) => axios.put(`${CANDIDATE_API_URL}/personal-info`, data, getAuthHeaders()),
  
  changePassword: (data) => axios.post(`${CANDIDATE_API_URL}/change-password`, data, getAuthHeaders()),

  addEducation: (data) => axios.post(`${PROFILE_API_URL}/education`, data, getAuthHeaders()),
  deleteEducation: (id) => axios.delete(`${PROFILE_API_URL}/education/${id}`, getAuthHeaders()),

  addExperience: (data) => axios.post(`${PROFILE_API_URL}/experience`, data, getAuthHeaders()),
  deleteExperience: (id) => axios.delete(`${PROFILE_API_URL}/experience/${id}`, getAuthHeaders()),

  addSkill: (data) => axios.post(`${PROFILE_API_URL}/skill`, data, getAuthHeaders()),
  deleteSkill: (id) => axios.delete(`${PROFILE_API_URL}/skill/${id}`, getAuthHeaders()),

  uploadCv: (formData) => axios.post(`${PROFILE_API_URL}/cv`, formData, {
    headers: { ...getAuthHeaders().headers, 'Content-Type': 'multipart/form-data' },
  }),
  deleteDocument: (id) => axios.delete(`${PROFILE_API_URL}/document/${id}`, getAuthHeaders()),
};

export default candidateProfileService;