import { useState, useEffect } from 'react';
import authService from './AuthService';
import { jwtDecode } from 'jwt-decode';

const useAuth = () => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(undefined);

  useEffect(() => {
    // const token = localStorage.getItem('token');
    // if (token) {
    //   const decodedToken = jwtDecode(token);
    //   const user = getUserFromToken(decodedToken);
    //   setUser(user);
    //   authService.setToken(token);
    //   setIsAuthenticated(true);
    //   return;
    // }
    // setIsAuthenticated(false);

    const token = localStorage.getItem('token');
    if (token) {
      authService.setToken(token);
    } else {
      setIsAuthenticated(false);
      return;
    }
    const user = getUser();

    if (user) {
      setIsAuthenticated(true);
    } else {
      setIsAuthenticated(false);
    }
  }, []);

  const handleAuthSuccess = (token) => {
    authService.setToken(token);
    localStorage.setItem('token', token);
    setIsAuthenticated(true);
  };

  const login = async (email, password) => {
    setLoading(true);
    setError(null);

    try {
      const { token } = await authService.login(email, password);
      handleAuthSuccess(token);
    } catch (err) {
      setError(err.response.data.message);
    } finally {
      setLoading(false);
    }
  };

  const register = async (username, password) => {
    setLoading(true);
    setError(null);

    try {
      const { token } = await authService.register(username, password);
      handleAuthSuccess(token);
    } catch (err) {
      setError(err.response.data.message);
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    setUser(null);
    authService.setToken(null);
    localStorage.removeItem('token');
    authService.logout();
    setIsAuthenticated(false);
  };

  const getUser = async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await authService.getUser();
      console.log(data);
      setUser(data);
    } catch (err) {
    } finally {
      setLoading(false);
    }
  };

  return { user, loading, error, login, register, logout, isAuthenticated };
};

export default useAuth;
