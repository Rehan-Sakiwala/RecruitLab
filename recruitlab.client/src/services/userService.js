import axios from 'axios';
import { getAuthHeaders } from '../utils/authHeaders';

const API_URL = 'https://localhost:7100/api/User';

const userService = {
  getInterviewers: async () => {
    const response = await axios.get(`${API_URL}/interviewers`, {
      ...getAuthHeaders()
    });
    return response.data;
  },

  getStaff: async () => {
    const response = await axios.get(`${API_URL}/staff`, {
      ...getAuthHeaders()
    });
    return response.data;
  }
};

export default userService;