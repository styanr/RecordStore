import { Navigate } from 'react-router-dom';
import useAuth from '../auth/useAuth';
import { useEffect } from 'react';

const ProtectedAdminRoute = ({ element }) => {
  const { isAuthenticated, user } = useAuth();

  if (isAuthenticated === undefined) {
    return <div>Loading...</div>;
  } else if (isAuthenticated && user.role === 'admin') {
    return element;
  } else {
    return <Navigate to='/login' />;
  }
};

export default ProtectedAdminRoute;
