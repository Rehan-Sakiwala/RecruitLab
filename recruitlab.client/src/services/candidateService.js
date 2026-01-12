import axios from 'axios';
import { getAuthHeaders } from '../utils/authHeaders';

const API_URL = 'https://localhost:7100/api/Candidate';

const candidateService = {
  getAllCandidates: async (page = 1, pageSize = 10, search = '') => {
    const params = { page, pageSize };
    if (search) params.search = search;
    
    const response = await axios.get(API_URL, { 
      ...getAuthHeaders(), 
      params 
    });
    
    return response.data;
  },

  getCandidateById: async (id) => {
    const response = await axios.get(`${API_URL}/${id}`, {
      ...getAuthHeaders()
    });
    return response.data;
  },
  
  deleteCandidate: async (id) => {
    await axios.delete(`${API_URL}/${id}`, {
      ...getAuthHeaders()
    });
  }
};

export default candidateService;