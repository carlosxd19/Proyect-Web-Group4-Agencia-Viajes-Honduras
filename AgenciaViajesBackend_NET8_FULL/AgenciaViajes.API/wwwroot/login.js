// login.js — Login con Firebase (Email/Password)
import { initializeApp } from "https://www.gstatic.com/firebasejs/10.12.2/firebase-app.js";
import {
  getAuth,
  setPersistence,
  browserLocalPersistence,
  signInWithEmailAndPassword
} from "https://www.gstatic.com/firebasejs/10.12.2/firebase-auth.js";

// ⛔ Pega aquí la configuración WEB de tu app (debe empezar con AIza…)
const firebaseConfig = {
  apiKey: "AIzaSyCHR9Q1Lkj4GyuN8-I8yb6ehGZJzmTAlQE",
  authDomain: "interviajes-af2ed.firebaseapp.com",
  projectId: "interviajes-af2ed",
  storageBucket: "interviajes-af2ed.firebasestorage.app",
  messagingSenderId: "410729345085",
  appId: "1:410729345085:web:a008eea84602b53bf2fa54"
};

const app = initializeApp(firebaseConfig);
const auth = getAuth(app);

// Mantener sesión iniciada en el navegador
await setPersistence(auth, browserLocalPersistence);

// DOM
const form = document.getElementById("loginForm");
const emailEl = document.getElementById("username");   // aquí va el correo
const passEl = document.getElementById("password");
const btn = document.getElementById("loginBtn");
const errorEl = document.getElementById("errorBox");

function showError(msg) {
  if (errorEl) {
    errorEl.textContent = msg;
    errorEl.style.display = "block";
  } else {
    alert(msg);
  }
}
function hideError() {
  if (errorEl) {
    errorEl.textContent = "";
    errorEl.style.display = "none";
  }
}

form.addEventListener("submit", async (e) => {
  e.preventDefault();
  hideError();
  btn.disabled = true;

  const email = (emailEl.value || "").trim().toLowerCase();
  const password = passEl.value || "";

  if (!email || !password) {
    showError("Ingresa tu correo y contraseña.");
    btn.disabled = false;
    return;
  }

  try {
    await signInWithEmailAndPassword(auth, email, password);
    // Redirige al entrar
    window.location.href = "/swagger/index.html"; // cámbialo si quieres
  } catch (err) {
    const code = err.code || "";
    let msg = "No se pudo iniciar sesión.";
    if (code === "auth/invalid-email") msg = "Correo inválido. Usa el mismo correo con el que te registraste.";
    else if (code === "auth/user-not-found") msg = "No existe una cuenta con ese correo.";
    else if (code === "auth/wrong-password") msg = "Contraseña incorrecta.";
    else if (code === "auth/invalid-api-key") msg = "apiKey inválida: revisa tu configuración Web.";
    else if (code === "auth/network-request-failed") msg = "Fallo de red. Verifica tu conexión.";
    showError(msg);
    console.error("Firebase login error:", err);
    btn.disabled = false;
  }
});
