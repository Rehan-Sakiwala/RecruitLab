import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import axios from 'axios';

const API_URL = 'https://localhost:7100/api';

// only open jobs
export const fetchOpenJobs = createAsyncThunk('jobs/fetchOpen', async () => {
  const response = await axios.get(`${API_URL}/Job`);
  return response.data.filter(job => job.status === 1 || job.statusName === 'Open');
});

const jobSlice = createSlice({
  name: 'jobs',
  initialState: {
    list: [],
    loading: false,
    error: null,
    currentPage: 1,
    jobsPerPage: 6, // Showing 6 cards per page looks good on grid
  },
  reducers: {
    setCurrentPage: (state, action) => {
      state.currentPage = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchOpenJobs.pending, (state) => {
        state.loading = true;
      })
      .addCase(fetchOpenJobs.fulfilled, (state, action) => {
        state.loading = false;
        state.list = action.payload;
      })
      .addCase(fetchOpenJobs.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message;
      });
  },
});

export const { setCurrentPage } = jobSlice.actions;
export default jobSlice.reducer;