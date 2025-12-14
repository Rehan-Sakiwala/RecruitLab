import axios from 'axios';
import { getAuthHeaders } from '../utils/authHeaders';

const API_URL = 'https://localhost:7100/api/Interview';

const interviewService = {
  getMyInterviews: () => axios.get(`${API_URL}/my-tasks`, getAuthHeaders()),
  submitFeedback: (data) => axios.post(`${API_URL}/feedback`, data, getAuthHeaders()),
};

export default interviewService;