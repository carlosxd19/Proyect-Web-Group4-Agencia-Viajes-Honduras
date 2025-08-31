document.getElementById("loginForm").addEventListener("submit", function (event) {
  event.preventDefault();

  const user = document.getElementById("username").value;
  const pass = document.getElementById("password").value;
  const message = document.getElementById("message");

  // Usuario y contraseña de ejemplo
  if (user === "admin" && pass === "1234") {
    message.style.color = "lime";
    message.textContent = "✅ Inicio de sesión exitoso";
  } else {
    message.style.color = "red";
    message.textContent = "❌ Usuario o contraseña incorrectos";
  }
});
