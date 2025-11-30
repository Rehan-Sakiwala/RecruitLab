import { configureStore } from '@reduxjs/toolkit';
import jobReducer from './features/jobs/jobSlice';
import authReducer from './features/auth/authSlice';

export const store = configureStore({
  reducer: {
    jobs: jobReducer,
    auth: authReducer,
  },
});