import { useContext } from 'react';
import { AuthContext } from '../AuthContext';

const useAuth = () => {
  const { user, loading, error, login, register, logout, isAuthenticated } =
    useContext(AuthContext);

  return { user, loading, error, login, register, logout, isAuthenticated };
};

export default useAuth;
