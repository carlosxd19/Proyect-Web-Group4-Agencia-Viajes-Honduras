// register.js — Registro con Firebase Auth + guardado en Firestore
import { initializeApp } from "https://www.gstatic.com/firebasejs/10.12.2/firebase-app.js";
import { getAuth, createUserWithEmailAndPassword, sendEmailVerification, signInWithEmailAndPassword } from "https://www.gstatic.com/firebasejs/10.12.2/firebase-auth.js";
import { getFirestore, doc, setDoc, serverTimestamp } from "https://www.gstatic.com/firebasejs/10.12.2/firebase-firestore.js";

// 1) Pega aquí tu config real desde Firebase Console (Proyecto: interviajes-af2ed)
const firebaseConfig = {
  apiKey: "AIzaSyCHR9Q1Lkj4GyuN8-I8yb6ehGZJzmTAlQE",
  authDomain: "interviajes-af2ed.firebaseapp.com",
  projectId: "interviajes-af2ed",
  storageBucket: "interviajes-af2ed.firebasestorage.app",
  messagingSenderId: "410729345085",
  appId: "1:410729345085:web:a008eea84602b53bf2fa54"
};

// 2) Inicializa Firebase, Auth y Firestore
const app = initializeApp(firebaseConfig);
const auth = getAuth(app);
const db = getFirestore(app);

// --- Helpers UI ---
const form = document.getElementById("registerForm");
const btn = document.getElementById("registerBtn");
const okBox = document.getElementById("okBox");
const errorBox = document.getElementById("errorBox");

function showError(msg) {
  errorBox.textContent = msg;
  errorBox.style.display = "block";
}
function clearError() {
  errorBox.textContent = "";
  errorBox.style.display = "none";
}
function showOK(msg) {
  okBox.textContent = msg;
  okBox.style.display = "block";
}
function clearOK() {
  okBox.textContent = "";
  okBox.style.display = "none";
}

// 3) Manejador de registro
form.addEventListener("submit", async (e) => {
  e.preventDefault();
  clearError();
  clearOK();
  btn.disabled = true; // placeholder to ensure disabled; will fix below
});

const fullname = (document.getElementById("fullname").value || "").trim();
const email = (document.getElementById("email").value || "").trim().toLowerCase();
const password = (document.getElementById("password").value || "");
const confirm = (document.getElementById("confirm").value || "");
const terms = document.getElementById("terms").checked;

// Validaciones básicas en cliente
if (!terms) {
  showError("Debes aceptar los términos y condiciones.");
  btn.disabled = false;
  return;
}
if (password !== confirm) {
  showError("Las contraseñas no coinciden.");
  btn.disabled = false;
  return;
}

try {
  // 4) Crea usuario en Auth
  const cred = await createUserWithEmailAndPassword(auth, email, password);
  const uid = cred.user.uid;

  // 5) Guarda perfil en Firestore (/users/{uid})
  const profile = {
    uid,
    fullname,
    email,
    role: "user",
    createdAt: serverTimestamp(),
    provider: "password"
  };
  await setDoc(doc(db, "users", uid), profile, { merge: true });

  // 6) (Opcional) Envía verificación de correo
  try { await sendEmailVerification(cred.user); } catch { }

  // 7) Autentica (ya queda firmado) y redirige
  showOK("¡Cuenta creada con éxito! Redirigiendo…");
  // Si quieres forzar login explícito:
  // await signInWithEmailAndPassword(auth, email, password);
  setTimeout(() => { window.location.href = "/swagger/index.html"; }, 800);

} catch (err) {
  console.error(err);
  let msg = "Error al registrar.";
  if (err?.code) {
    switch (err.code) {
      case "auth/email-already-in-use": msg = "El correo ya está en uso."; break;
      case "auth/invalid-email": msg = "Correo inválido."; break;
      case "auth/weak-password": msg = "La contraseña es muy débil (mínimo 6 caracteres)."; break;
      default: msg = "Error: " + err.code;
    }
  }
  showError(msg);
  btn.disabled = false;
}
});
