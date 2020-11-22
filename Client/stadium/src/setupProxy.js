const { createProxyMiddleware } = require('http-proxy-middleware');
module.exports = function(app) {
  app.use(
    'admin/auth/signin',
    createProxyMiddleware({
      target: 'http://localhost:5000/',
      secure: false,
      changeOrigin: true,
    })
  );
};