import axios from 'axios';
import { getAuthHeaders } from '../utils/authHeaders';

const API_URL = 'https://localhost:7100/api';

const applicationService = {
  // Candidate Endpoints
  applyForJob: (jobOpeningId) => axios.post(`${API_URL}/Application/apply`, { jobOpeningId }, getAuthHeaders()),
  getMyApplications: () => axios.get(`${API_URL}/Application/my-applications`, getAuthHeaders()),
  
  // Recruiter Endpoints
  getApplicationsForJob: (jobId) => axios.get(`${API_URL}/Application/job/${jobId}`, getAuthHeaders()),
  updateStatus: (id, data) => axios.put(`${API_URL}/Application/${id}/status`, data, getAuthHeaders()),
  assignReviewer: (id, reviewerId) => axios.put(`${API_URL}/Application/${id}/assign-reviewer`, { reviewerId }, getAuthHeaders()),
};

export default applicationService;