// app.js (módulo ES) — Login + conexión a Firebase (inicialización simple)
import { initializeApp } from "https://www.gstatic.com/firebasejs/10.12.2/firebase-app.js";
// (Opcional) Si más adelante quiere Auth/Firestore, descomente los imports que necesite:
// import { getAuth } from "https://www.gstatic.com/firebasejs/10.12.2/firebase-auth.js";
// import { getFirestore } from "https://www.gstatic.com/firebasejs/10.12.2/firebase-firestore.js";

// TODO: Reemplace estos valores con los de su proyecto en Firebase Console
const firebaseConfig = {
  apiKey: "AlzaSyCHR9Q1Lkj4GyuN8-l8yb6ehGZJzmTAIQE",
  authDomain: "interviajes-af2ed.firebaseapp.com",
  projectId: "interviajes-af2ed",
  storageBucket: "interviajes-af2ed.appspot.com",
  messagingSenderId: "410729345085",
  appId: "1:410729345085:web:a008eea84602b53bf2fa54"
};

// Inicializa Firebase (sólo conexión)
const fbApp = initializeApp(firebaseConfig);
// const auth = getAuth(fbApp); // si lo necesita luego
// const db = getFirestore(fbApp); // si lo necesita luego

// -------- Login local para desarrollo --------
const DEFAULT_USER = { username: "cd_19", password: "Carlos2004" };
const saved = JSON.parse(localStorage.getItem("login_user") || "null");
const user = saved && saved.username && saved.password ? saved : DEFAULT_USER;

const form = document.getElementById("loginForm");
const errorBox = document.getElementById("errorBox");
const loginBtn = document.getElementById("loginBtn");

function showError(msg) {
  errorBox.textContent = msg;
  errorBox.style.display = "block";
}

function hideError() {
  errorBox.textContent = "";
  errorBox.style.display = "none";
}

form.addEventListener("submit", (e) => {
  e.preventDefault();
  hideError();
  loginBtn.disabled = true;

  const username = (document.getElementById("username").value || "").trim();
  const password = (document.getElementById("password").value || "");

  if (username === user.username && password === user.password) {
    localStorage.setItem("last_login_user", JSON.stringify({ username }));
    window.location.href = "/swagger/index.html";
  } else {
    showError("Usuario o contraseña incorrectos. Intente nuevamente.");
    loginBtn.disabled = false;
  }
});
