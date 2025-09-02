document.getElementById("loginForm").addEventListener("submit", function (event) {
  event.preventDefault();

  const user = document.getElementById("username").value;
  const pass = document.getElementById("password").value;
  const message = document.getElementById("message");

  // Usuario y contraseña de ejemplo
  if (user === "cd_19" && pass === "Carlos2004") {
    message.style.color = "lime";
    message.textContent = "✅ Inicio de sesión exitoso";

    // Espera un momento para mostrar el mensaje y luego redirige a Swagger
    setTimeout(() => {
      window.location.href = "https://localhost:7053/swagger";
      // Cambia el puerto si tu API usa otro
    }, 800);

  } else {
    message.style.color = "red";
    message.textContent = "❌ Usuario o contraseña incorrectos";
  }
});
