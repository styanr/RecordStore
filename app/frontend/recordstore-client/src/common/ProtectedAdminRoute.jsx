import { Navigate } from 'react-router-dom';
import { useEffect } from 'react';
import useAuth from '../hooks/useAuth';
const ProtectedAdminRoute = ({ element }) => {
  const { isAuthenticated, user, loading } = useAuth();

  useEffect(() => {
    console.log('ProtectedAdminRoute');
    console.log(loading, isAuthenticated, user);
  }, [loading]);

  if (isAuthenticated === undefined || loading) {
    return <div>Loading...</div>;
  }

  if (isAuthenticated && user && user.role === 'admin') {
    return element;
  }

  return <Navigate to='/' />;
};

export default ProtectedAdminRoute;
