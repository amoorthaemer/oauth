NOTE: This Cert-Manager is for LetsEncrypt DNS-01 certificate requests

Install cert-manager
=============================================================================================================

# Linux Bash
helm install \
  cert-manager jetstack/cert-manager \
  --namespace cert-manager \
  --version v1.11.0 \
  --set installCRDs=true \
  --set "extraArgs={--dns01-recursive-nameservers-only,--dns01-recursive-nameservers=8.8.8.8:53\,1.1.1.1:53}"

# CMD proompt
helm install ^
  cert-manager jetstack/cert-manager ^
  --namespace cert-manager ^
  --version v1.11.0 ^
  --set installCRDs=true ^
  --set "extraArgs={--dns01-recursive-nameservers-only,--dns01-recursive-nameservers=8.8.8.8:53\,1.1.1.1:53}"

# POWERSHELL
helm install `
  cert-manager jetstack/cert-manager `
  --namespace cert-manager `
  --create-namespace `
  --version v1.11.0 `
  --set installCRDs=true `
  --set "extraArgs={--dns01-recursive-nameservers-only,--dns01-recursive-nameservers=8.8.8.8:53\,1.1.1.1:53}"
