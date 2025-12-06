import React, { useEffect, useState } from "react";
import candidateProfileService from "../../services/candidateProfileService";
import jobService from "../../services/jobService";
import {
  User,
  Briefcase,
  GraduationCap,
  Code,
  FileText,
  Plus,
  Trash2,
  Upload,
  Loader2,
  Edit2,
  Lock,
  X,
  Save,
  Download,
} from "lucide-react";

const CandidateProfile = () => {
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [tab, setTab] = useState("overview");
  const [skillsList, setSkillsList] = useState([]);
  const [isEdit, setIsEdit] = useState(false);

  // Forms State
  const [pForm, setPForm] = useState({});
  const [pass, setPass] = useState({ currentPassword: "", newPassword: "" });
  const [edu, setEdu] = useState({
    schoolName: "",
    degree: "",
    fieldOfStudy: "",
    startDate: "",
    endDate: "",
    isCurrent: false,
  });
  const [exp, setExp] = useState({
    companyName: "",
    position: "",
    location: "",
    startDate: "",
    endDate: "",
    isCurrent: false,
    responsibilities: "",
  });
  const [skill, setSkill] = useState({
    skillId: "",
    level: 1,
    yearsOfExperience: 0,
  });

  const load = async () => {
    try {
      const [res, sRes] = await Promise.all([
        candidateProfileService.getMyProfile(),
        jobService.getAllSkills(),
      ]);
      setProfile(res.data);
      setPForm(res.data);
      setSkillsList(sRes.data);
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  // Actions
  const saveInfo = async (e) => {
    e.preventDefault();
    await candidateProfileService.updatePersonalInfo(pForm);
    setIsEdit(false);
    load();
  };
  const savePass = async (e) => {
    e.preventDefault();
    try {
      await candidateProfileService.changePassword(pass);
      alert("Password updated");
      setPass({ currentPassword: "", newPassword: "" });
    } catch (err) {
      alert("Failed");
    }
  };

  const add = async (api, data, setter, reset) => {
    e.preventDefault();
    await api({ ...data, candidateId: profile.id });
    setter(reset);
    load();
  };
  const del = async (type, id) => {
    if (confirm("Delete?")) {
      const map = {
        edu: "deleteEducation",
        exp: "deleteExperience",
        skill: "deleteSkill",
        doc: "deleteDocument",
      };
      await candidateProfileService[map[type]](id);
      load();
    }
  };

  const upload = async (e) => {
    if (!e.target.files[0]) return;
    const fd = new FormData();
    fd.append("File", e.target.files[0]);
    fd.append("CandidateId", profile.id);
    await candidateProfileService.uploadCv(fd);
    load();
  };

  if (loading || !profile)
    return (
      <div className="p-20 text-center">
        <Loader2 className="animate-spin inline text-blue-600" /> Loading...
      </div>
    );

  return (
    <div className="max-w-5xl mx-auto my-6 bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
      {/* Header */}
      <div className="p-8 bg-gray-50 border-b relative">
        <button
          onClick={() => setIsEdit(!isEdit)}
          className="absolute top-6 right-6 text-gray-400 hover:text-blue-600"
        >
          {isEdit ? <X /> : <Edit2 size={18} />}
        </button>
        {isEdit ? (
          <form onSubmit={saveInfo} className="grid grid-cols-2 gap-3 max-w-xl">
            <Input
              val={pForm.firstName}
              onChange={(v) => setPForm({ ...pForm, firstName: v })}
              ph="First Name"
            />
            <Input
              val={pForm.lastName}
              onChange={(v) => setPForm({ ...pForm, lastName: v })}
              ph="Last Name"
            />
            <Input
              val={pForm.phoneNumber}
              onChange={(v) => setPForm({ ...pForm, phoneNumber: v })}
              ph="Phone"
            />
            <Input
              val={pForm.linkedInProfile}
              onChange={(v) => setPForm({ ...pForm, linkedInProfile: v })}
              ph="LinkedIn"
            />
            <button className="btn-primary col-span-2">Save</button>
          </form>
        ) : (
          <div className="flex items-center gap-6">
            <div className="w-16 h-16 bg-blue-600 rounded-full flex items-center justify-center text-white text-2xl font-bold">
              {profile.firstName[0]}
            </div>
            <div>
              <h1 className="text-2xl font-bold text-gray-900">
                {profile.firstName} {profile.lastName}
              </h1>
              <p className="text-gray-500 text-sm">
                {profile.email} • {profile.phoneNumber}
              </p>
            </div>
          </div>
        )}
      </div>

      {/* Tabs */}
      <div className="flex border-b overflow-x-auto">
        {[
          "overview",
          "experience",
          "education",
          "skills",
          "documents",
          "security",
        ].map((t) => (
          <button
            key={t}
            onClick={() => setTab(t)}
            className={`px-6 py-3 text-sm font-medium capitalize border-b-2 ${
              tab === t
                ? "border-blue-600 text-blue-600"
                : "border-transparent text-gray-500"
            }`}
          >
            {t}
          </button>
        ))}
      </div>

      <div className="p-8 min-h-[400px]">
        {tab === "overview" && (
          <div className="space-y-4">
            <h3 className="font-bold text-lg">Summary</h3>
            <p className="text-gray-600">{profile.notes || "No summary."}</p>
            <div className="flex gap-4">
              <Stat label="Current Salary" val={profile.currentSalary} />
              <Stat label="Expected Salary" val={profile.expectedSalary} />
            </div>
          </div>
        )}

        {tab === "experience" && (
          <Section
            items={profile.experienceHistory}
            renderItem={(i) => (
              <Item
                title={i.position}
                sub={i.companyName}
                date={i}
                onDelete={() => del("exp", i.id)}
                desc={i.responsibilities}
              />
            )}
          >
            <form
              onSubmit={(e) =>
                add(candidateProfileService.addExperience, exp, setExp, {
                  companyName: "",
                  position: "",
                  startDate: "",
                  endDate: "",
                  isCurrent: false,
                  responsibilities: "",
                })
              }
              className="grid grid-cols-2 gap-3 bg-gray-50 p-4 rounded"
            >
              <Input
                val={exp.position}
                onChange={(v) => setExp({ ...exp, position: v })}
                ph="Job Title"
                req
              />
              <Input
                val={exp.companyName}
                onChange={(v) => setExp({ ...exp, companyName: v })}
                ph="Company"
                req
              />
              <Input
                type="date"
                val={exp.startDate}
                onChange={(v) => setExp({ ...exp, startDate: v })}
                req
              />
              <Input
                type="date"
                val={exp.endDate}
                onChange={(v) => setExp({ ...exp, endDate: v })}
                dis={exp.isCurrent}
              />
              <label className="text-sm flex items-center gap-2">
                <input
                  type="checkbox"
                  checked={exp.isCurrent}
                  onChange={(e) =>
                    setExp({ ...exp, isCurrent: e.target.checked })
                  }
                />{" "}
                Current
              </label>
              <textarea
                className="input col-span-2"
                rows="2"
                placeholder="Responsibilities"
                value={exp.responsibilities}
                onChange={(e) =>
                  setExp({ ...exp, responsibilities: e.target.value })
                }
              />
              <button className="btn-primary w-fit">Add</button>
            </form>
          </Section>
        )}

        {tab === "education" && (
          <Section
            items={profile.educationHistory}
            renderItem={(i) => (
              <Item
                title={i.schoolName}
                sub={`${i.degree}, ${i.fieldOfStudy}`}
                date={i}
                onDelete={() => del("edu", i.id)}
              />
            )}
          >
            <form
              onSubmit={(e) =>
                add(candidateProfileService.addEducation, edu, setEdu, {
                  schoolName: "",
                  degree: "",
                  fieldOfStudy: "",
                  startDate: "",
                  endDate: "",
                  isCurrent: false,
                })
              }
              className="grid grid-cols-2 gap-3 bg-gray-50 p-4 rounded"
            >
              <Input
                val={edu.schoolName}
                onChange={(v) => setEdu({ ...edu, schoolName: v })}
                ph="School"
                req
              />
              <Input
                val={edu.degree}
                onChange={(v) => setEdu({ ...edu, degree: v })}
                ph="Degree"
                req
              />
              <Input
                val={edu.fieldOfStudy}
                onChange={(v) => setEdu({ ...edu, fieldOfStudy: v })}
                ph="Field"
                req
              />
              <div className="flex gap-2">
                <Input
                  type="date"
                  val={edu.startDate}
                  onChange={(v) => setEdu({ ...edu, startDate: v })}
                  req
                />
                <Input
                  type="date"
                  val={edu.endDate}
                  onChange={(v) => setEdu({ ...edu, endDate: v })}
                />
              </div>
              <button className="btn-primary w-fit">Add</button>
            </form>
          </Section>
        )}

        {tab === "skills" && (
          <Section
            items={profile.candidateSkills}
            renderItem={(i) => (
              <div className="flex justify-between p-3 border rounded bg-gray-50">
                <span>
                  {i.skillName} ({i.levelName}, {i.yearsOfExperience}y)
                </span>
                <button
                  onClick={() => del("skill", i.id)}
                  className="text-red-400"
                >
                  <Trash2 size={16} />
                </button>
              </div>
            )}
            grid
          >
            <form
              onSubmit={(e) =>
                add(candidateProfileService.addSkill, skill, setSkill, {
                  skillId: "",
                  level: 1,
                  yearsOfExperience: 0,
                })
              }
              className="flex gap-2 bg-gray-50 p-4 rounded mt-4"
            >
              <select
                className="input"
                value={skill.skillId}
                onChange={(e) =>
                  setSkill({ ...skill, skillId: e.target.value })
                }
              >
                <option value="">Skill...</option>
                {skillsList.map((s) => (
                  <option key={s.id} value={s.id}>
                    {s.name}
                  </option>
                ))}
              </select>
              <select
                className="input w-32"
                value={skill.level}
                onChange={(e) => setSkill({ ...skill, level: e.target.value })}
              >
                <option value="1">Beginner</option>
                <option value="2">Intermediate</option>
                <option value="3">Expert</option>
              </select>
              <Input
                type="number"
                val={skill.yearsOfExperience}
                onChange={(v) => setSkill({ ...skill, yearsOfExperience: v })}
                ph="Yrs"
                className="w-20"
              />
              <button className="btn-primary">
                <Plus />
              </button>
            </form>
          </Section>
        )}

        {tab === "documents" && (
          <Section
            items={profile.documents}
            renderItem={(i) => (
              <div className="flex justify-between p-3 border rounded items-center">
                <div className="flex gap-2">
                  <FileText /> {i.fileName}
                </div>
                <div className="flex gap-2">
                  <a
                    href={`https://localhost:7100/${i.filePath}`}
                    target="_blank"
                    className="text-blue-600"
                  >
                    <Download size={16} />
                  </a>
                  <button
                    onClick={() => del("doc", i.id)}
                    className="text-red-500"
                  >
                    <Trash2 size={16} />
                  </button>
                </div>
              </div>
            )}
          >
            <div className="border-2 border-dashed p-8 text-center rounded relative cursor-pointer hover:bg-gray-50">
              <Upload className="mx-auto text-gray-400 mb-2" />
              <p>Click to Upload CV</p>
              <input
                type="file"
                onChange={upload}
                className="absolute inset-0 opacity-0 cursor-pointer"
              />
            </div>
          </Section>
        )}

        {tab === "security" && (
          <form onSubmit={savePass} className="space-y-4 max-w-sm">
            <h3 className="font-bold flex gap-2">
              <Lock /> Change Password
            </h3>
            <Input
              type="password"
              ph="Current Password"
              val={pass.currentPassword}
              onChange={(v) => setPass({ ...pass, currentPassword: v })}
              req
            />
            <Input
              type="password"
              ph="New Password"
              val={pass.newPassword}
              onChange={(v) => setPass({ ...pass, newPassword: v })}
              req
            />
            <button className="btn-primary bg-red-600 hover:bg-red-700">
              Update
            </button>
          </form>
        )}
      </div>
      <style>{`.input { @apply border border-gray-300 rounded px-3 py-2 w-full text-sm focus:ring-2 focus:ring-blue-500 outline-none; } .btn-primary { @apply bg-blue-600 text-white px-4 py-2 rounded text-sm font-medium hover:bg-blue-700 transition; }`}</style>
    </div>
  );
};

// Micro Components
const Section = ({ items, renderItem, children, grid }) => (
  <div className="space-y-4">
    <div className={grid ? "grid grid-cols-2 gap-4" : "space-y-3"}>
      {items.map((i) => (
        <div key={i.id}>{renderItem(i)}</div>
      ))}
    </div>
    {children}
  </div>
);
const Input = ({
  val,
  onChange,
  ph,
  type = "text",
  req,
  dis,
  className = "",
}) => (
  <input
    type={type}
    placeholder={ph}
    required={req}
    disabled={dis}
    className={`input ${className}`}
    value={val}
    onChange={(e) => onChange(e.target.value)}
  />
);
const Stat = ({ label, val }) => (
  <div className="p-4 border rounded flex-1">
    <span className="text-xs text-gray-500 uppercase">{label}</span>
    <div className="font-medium text-lg">{val ? `INR ${val}` : "N/A"}</div>
  </div>
);
const Item = ({ title, sub, date, desc, onDelete }) => (
  <div className="flex justify-between items-start p-4 border rounded hover:bg-gray-50">
    <div>
      <h4 className="font-bold">{title}</h4>
      <p className="text-blue-600 text-sm">{sub}</p>
      <p className="text-xs text-gray-500 mt-1">
        {new Date(date.startDate).getFullYear()} -{" "}
        {date.isCurrent ? "Present" : new Date(date.endDate).getFullYear()}
      </p>
      {desc && <p className="text-sm mt-1 text-gray-600">{desc}</p>}
    </div>
    <button onClick={onDelete} className="text-gray-300 hover:text-red-500">
      <Trash2 size={18} />
    </button>
  </div>
);

export default CandidateProfile;
