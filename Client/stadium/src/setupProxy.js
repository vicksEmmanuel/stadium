const { createProxyMiddleware } = require('http-proxy-middleware');
module.exports = function(app) {
  app.use(
    '/v3/transactions',
    createProxyMiddleware({
      target: 'https://api.flutterwave.com',
      secure: false,
      changeOrigin: true,
    })
  );
};