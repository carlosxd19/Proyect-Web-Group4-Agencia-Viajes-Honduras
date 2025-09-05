// register.js
// Usa módulos del SDK Web (cliente). NO coloques aquí la clave del Admin SDK.

import { initializeApp } from "https://www.gstatic.com/firebasejs/10.12.2/firebase-app.js";
import {
  getAuth,
  createUserWithEmailAndPassword,
  updateProfile
} from "https://www.gstatic.com/firebasejs/10.12.2/firebase-auth.js";
import {
  getFirestore,
  doc,
  setDoc,
  serverTimestamp
} from "https://www.gstatic.com/firebasejs/10.12.2/firebase-firestore.js";

// ⛔ Reemplaza ESTO con tu configuración de app web (Project settings → Your apps → Web)
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
const db = getFirestore(app);

const $ = (sel) => document.querySelector(sel);
const form = $("#registerForm");
const msgError = $("#msgError");
const msgOk = $("#msgOk");

function showError(text) {
  msgOk.style.display = "none";
  msgError.textContent = text;
  msgError.style.display = "block";
}

function showOk(text) {
  msgError.style.display = "none";
  msgOk.textContent = text;
  msgOk.style.display = "block";
}

form.addEventListener("submit", async (e) => {
  e.preventDefault();

  const nombre = $("#nombre").value.trim();
  const email = $("#email").value.trim().toLowerCase();
  const telefono = $("#telefono").value.trim();
  const pass = $("#password").value;
  const pass2 = $("#password2").value;

  if (!nombre) return showError("Ingresa tu nombre.");
  if (pass !== pass2) return showError("Las contraseñas no coinciden.");
  if (pass.length < 6) return showError("La contraseña debe tener al menos 6 caracteres.");

  try {
    // 1) Crear usuario en Authentication
    const cred = await createUserWithEmailAndPassword(auth, email, pass);

    // 2) Poner displayName (opcional)
    await updateProfile(cred.user, { displayName: nombre });

    // 3) Guardar datos del usuario en Firestore
    const uid = cred.user.uid;
    await setDoc(doc(db, "usuarios", uid), {
      uid,
      nombre,
      email,
      telefono: telefono || null,
      rol: "cliente",        // puedes cambiarlo si manejas roles
      creadoEn: serverTimestamp(),
      proveedor: "password"  // auth provider
    });

    showOk("¡Cuenta creada con éxito! Redirigiendo…");
    // 4) Redirige a login o dashboard
    setTimeout(() => {
      window.location.href = "index.html"; // o "dashboard.html"
    }, 900);
  } catch (err) {
    // Manejo de errores comunes
    const code = err.code || "";
    let txt = "Ocurrió un error al registrar.";

    if (code === "auth/email-already-in-use") txt = "Ese correo ya está en uso.";
    else if (code === "auth/invalid-email") txt = "Correo inválido.";
    else if (code === "auth/weak-password") txt = "La contraseña es muy débil.";
    else if (code === "permission-denied") txt = "No tienes permiso para escribir en la base de datos.";
    else if (err.message) txt = err.message;

    showError(txt);
    console.error(err);
  }
});
