import { Navigate } from 'react-router-dom';
import useAuth from '../auth/useAuth';
import { useEffect } from 'react';

const ProtectedRoute = ({ element }) => {
  const { isAuthenticated } = useAuth();

  useEffect(() => {
    console.log(isAuthenticated);
  }, [isAuthenticated]);

  if (isAuthenticated === undefined) {
    return <div>Loading...</div>;
  } else if (isAuthenticated) {
    return element;
  } else {
    return <Navigate to='/login' />;
  }
};

export default ProtectedRoute;
