import { Navigate } from 'react-router-dom';
import useAuth from '../hooks/useAuth';
import { useEffect } from 'react';

const ProtectedRoute = ({ element }) => {
  const { isAuthenticated, loading } = useAuth();

  useEffect(() => {
    console.log('ProtectedRoute');
  }, []);

  if (loading) {
    return <div>Loading...</div>;
  }

  if (isAuthenticated) {
    return element;
  }

  return <Navigate to='/login' />;
};

export default ProtectedRoute;
