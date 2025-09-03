import { initializeApp } from "https://www.gstatic.com/firebasejs/9.22.0/firebase-app.js";
import { getAuth, signInWithEmailAndPassword } from "https://www.gstatic.com/firebasejs/9.22.0/firebase-auth.js";

const firebaseConfig = {
  apiKey: "AlzaSyCHR9Q1Lkj4GyuN8-l8yb6ehGZJzmTAIQE",
  authDomain: "interviajes-af2ed.firebaseapp.com",
  projectId: "interviajes-af2ed",
  storageBucket: "interviajes-af2ed.appspot.com",
  messagingSenderId: "410729345085",
  appId: "1:410729345085:web:a008eea84602b53bf2fa54"
};

const app = initializeApp(firebaseConfig);
const auth = getAuth(app);

document.getElementById("loginForm").addEventListener("submit", async function (event) {
  event.preventDefault();

  const email = document.getElementById("email").value.trim(); // asegurarse que sea el mismo id que registro
  const password = document.getElementById("password").value.trim();
  const message = document.getElementById("message");

  try {
    const userCredential = await signInWithEmailAndPassword(auth, email, password);
    const user = userCredential.user;

    message.style.color = "lime";
    message.textContent = "✅ Inicio de sesión exitoso";

    // Redirigir después de 0.8s
    setTimeout(() => {
      window.location.href = "https://localhost:7053/swagger";
    }, 800);

  } catch (error) {
    console.error("Error al iniciar sesión:", error);
    message.style.color = "red";
    message.textContent = `❌ ${error.message}`;
  }
});
