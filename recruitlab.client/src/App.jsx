import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Layout from './components/Layout';
import Login from './components/Login';
import Dashboard from './components/Dashboard';
import JobForm from './components/JobForm';
import './App.css';

function App() {
  return (
    <AuthProvider>
      <Router>
        <div className="App">
          <Routes>
            <Route path="/login" element={<Login />} />
            
            <Route path="/" element={
              <ProtectedRoute>
                <Layout>
                  <Navigate to="/dashboard" replace />
                </Layout>
              </ProtectedRoute>
            } />
            
            <Route path="/dashboard" element={
              <ProtectedRoute>
                <Layout>
                  <Dashboard />
                </Layout>
              </ProtectedRoute>
            } />
            
            <Route path="/jobs/create" element={
              <ProtectedRoute>
                <Layout>
                  <JobForm />
                </Layout>
              </ProtectedRoute>
            } />
            
            <Route path="/jobs/:id/edit" element={
              <ProtectedRoute>
                <Layout>
                  <JobForm isEdit={true} />
                </Layout>
              </ProtectedRoute>
            } />
            
            <Route path="/unauthorized" element={
              <div className="min-h-screen flex items-center justify-center bg-gray-50">
                <div className="max-w-md w-full space-y-8 text-center">
                  <div>
                    <h2 className="mt-6 text-3xl font-extrabold text-gray-900">
                      Access Denied
                    </h2>
                    <p className="mt-2 text-sm text-gray-600">
                      You don't have permission to access this page.
                    </p>
                  </div>
                  <div className="mt-8">
                    <a
                      href="/dashboard"
                      className="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                    >
                      Go to Dashboard
                    </a>
                  </div>
                </div>
              </div>
            } />
            
            <Route path="*" element={<Navigate to="/dashboard" replace />} />
          </Routes>
        </div>
      </Router>
    </AuthProvider>
  );
}

export default App;
