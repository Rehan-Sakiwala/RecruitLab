import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import axios from 'axios';

const API_URL = 'https://localhost:7100/api/Auth';

const token = localStorage.getItem('token');
const user = JSON.parse(localStorage.getItem('user'));

const initialState = {
  user: user ? user : null,
  token: token ? token : null,
  isError: false,
  isSuccess: false,
  isRegistered: false,
  isResetInitiated: false,
  isLoading: false,
  message: '',
};

// 1. Login User
export const login = createAsyncThunk('auth/login', async (userData, thunkAPI) => {
  try {
    const response = await axios.post(`${API_URL}/login`, userData);
    
    if (response.data) {
      localStorage.setItem('user', JSON.stringify(response.data));
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('role', response.data.role);
    }
    return response.data;
  } catch (error) {
    const message = extractErrorMessage(error);
    return thunkAPI.rejectWithValue(message);
  }
});

// 2. Register Candidate (Step 1: Triggers OTP)
export const register = createAsyncThunk('auth/register', async (userData, thunkAPI) => {
  try {
    const response = await axios.post(`${API_URL}/register-candidate`, userData);
    return response.data; 
  } catch (error) {
    const message = extractErrorMessage(error);
    return thunkAPI.rejectWithValue(message);
  }
});

// 3. Verify OTP (Step 2: Activates Account)
export const verifyOtp = createAsyncThunk('auth/verifyOtp', async (otpData, thunkAPI) => {
  try {
    const response = await axios.post(`${API_URL}/verify-otp`, otpData);
    return response.data;
  } catch (error) {
    const message = extractErrorMessage(error);
    return thunkAPI.rejectWithValue(message);
  }
});

// 4. Logout User
export const logout = createAsyncThunk('auth/logout', async () => {
  localStorage.removeItem('user');
  localStorage.removeItem('token');
  localStorage.removeItem('role');
});

// 5. Forgot Password (Step 1: Request Code)
export const forgotPassword = createAsyncThunk('auth/forgotPassword', async (email, thunkAPI) => {
  try {
    const response = await axios.post(`${API_URL}/forgot-password`, { email });
    return response.data;
  } catch (error) {
    const message = extractErrorMessage(error);
    return thunkAPI.rejectWithValue(message);
  }
});

// 6. Reset Password (Step 2: Verify Code & Set New Password)
export const resetPassword = createAsyncThunk('auth/resetPassword', async (resetData, thunkAPI) => {
  try {
    const response = await axios.post(`${API_URL}/reset-password`, resetData);
    return response.data;
  } catch (error) {
    const message = extractErrorMessage(error);
    return thunkAPI.rejectWithValue(message);
  }
});

const extractErrorMessage = (error) => {
  if (error.response && error.response.data) {
    if (error.response.data.errors) {
        return Object.values(error.response.data.errors).flat().join(', ');
    } 
    else if (error.response.data.message) {
        return error.response.data.message;
    }
    else if (error.response.data.title) {
        return error.response.data.title;
    }
  }
  return error.message || error.toString();
};

export const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    reset: (state) => {
      state.isLoading = false;
      state.isSuccess = false;
      state.isError = false;
      state.isRegistered = false;
      state.message = '';
    },
  },
  extraReducers: (builder) => {
    builder
      // --- LOGIN CASES ---
      .addCase(login.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(login.fulfilled, (state, action) => {
        state.isLoading = false;
        state.isSuccess = true;
        state.user = action.payload;
        state.token = action.payload.token;
      })
      .addCase(login.rejected, (state, action) => {
        state.isLoading = false;
        state.isError = true;
        state.message = action.payload;
        state.user = null;
        state.token = null;
      })

      // --- REGISTER CASES (Step 1) ---
      .addCase(register.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(register.fulfilled, (state) => {
        state.isLoading = false;
        state.isRegistered = true;
        state.message = "OTP sent successfully to your email.";
        state.isError = false;
      })
      .addCase(register.rejected, (state, action) => {
        state.isLoading = false;
        state.isError = true;
        state.message = action.payload;
      })

      // --- VERIFY OTP CASES (Step 2) ---
      .addCase(verifyOtp.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(verifyOtp.fulfilled, (state) => {
        state.isLoading = false;
        state.isSuccess = true; // Redirect to Login
        state.isRegistered = false; 
        state.message = "Email verified successfully.";
      })
      .addCase(verifyOtp.rejected, (state, action) => {
        state.isLoading = false;
        state.isError = true;
        state.message = action.payload;
      })

      // --- LOGOUT CASES ---
      .addCase(logout.fulfilled, (state) => {
        state.user = null;
        state.token = null;
        state.isSuccess = false;
      });
  },
});

export const { reset } = authSlice.actions;
export default authSlice.reducer;