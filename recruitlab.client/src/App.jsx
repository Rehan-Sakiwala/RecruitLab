import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import "./App.css";
import LandingPage from "./pages/LandingPage";
import LoginPage from "./pages/LoginPage";
import RecruiterLayout from "./layouts/RecruiterLayout";
import RecruiterDashboard from "./pages/recruiter/RecruiterDashboard";
import JobList from "./pages/recruiter/Jobs/JobList";
import CreateJob from "./pages/recruiter/Jobs/CreateJob";
import EditJob from "./pages/recruiter/Jobs/EditJob";
import CandidateLayout from "./layouts/CandidateLayout";
import CandidateDashboard from "./pages/candidate/CandidateDashboard";
import MyApplications from "./pages/candidate/MyApplications";
import CandidateProfile from "./pages/candidate/CandidateProfile";
import CandidateJobs from "./pages/candidate/CandidateJobs";
import InterviewerLayout from "./layouts/InterviewerLayout";
import InterviewerDashboard from "./pages/interviewer/InterviewerDashboard";
import FeedbackForm from "./pages/interviewer/FeedbackForm";

function App() {
  return (
    <Routes>
      <Route path="/" element={<LandingPage />} />
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/signup"
        element={
          <div className="p-10 text-center">Signup Page Coming Soon</div>
        }
      />
      {/* Recruiter Routes (Protected) */}
      <Route path="/recruiter" element={<RecruiterLayout />}>
        <Route path="dashboard" element={<RecruiterDashboard />} />
        <Route path="jobs" element={<JobList />} />
        <Route path="jobs/create" element={<CreateJob />} />
        <Route path="jobs/edit/:id" element={<EditJob />} />
        <Route
          path="jobs/:jobId/pipeline"
          element={<div className="p-10">Pipeline Board Coming Soon</div>}
        />
        {/* Placeholder Routes */}
        <Route path="candidates" element={<div>Candidates Database</div>} />
        <Route path="interviews" element={<div>Interviews Calendar</div>} />
      </Route>

      <Route path="/candidate" element={<CandidateLayout />}>
        <Route index element={<Navigate to="dashboard" replace />} />
        <Route path="dashboard" element={<CandidateDashboard />} />
        <Route path="jobs" element={<CandidateJobs />} />
        <Route path="applications" element={<MyApplications />} />
        <Route path="profile" element={<CandidateProfile />} />
      </Route>

      <Route path="/interviewer" element={<InterviewerLayout />}>
        <Route index element={<Navigate to="dashboard" replace />} />
        <Route path="dashboard" element={<InterviewerDashboard />} />
        <Route path="feedback/:interviewId" element={<FeedbackForm />} />
      </Route>
    </Routes>
  );
}

export default App;
