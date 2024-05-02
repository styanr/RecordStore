import { Navigate } from 'react-router-dom';
import { useEffect } from 'react';
import useAuth from '../hooks/useAuth';
const ProtectedAdminRoute = ({ element }) => {
  const { isAuthenticated, user, loading } = useAuth();

  if (loading) {
    return <div>Loading...</div>;
  }

  if (isAuthenticated && user && user.role === 'admin') {
    return element;
  }

  return <Navigate to='/' />;
};

export default ProtectedAdminRoute;
