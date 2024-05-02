import React, { createContext, useState, useEffect } from 'react';

import { useNavigate } from 'react-router-dom';

import authService from './hooks/AuthService';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const navigate = useNavigate();

  const [user, setUser] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(undefined);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    console.log('AuthContext useEffect');
    const token = localStorage.getItem('token');
    console.log(token);

    if (token) {
      authService.setToken(token);
      getUser()
        .then(() => {
          setIsAuthenticated(true);
          console.log('no error');
        })
        .catch(() => {
          setIsAuthenticated(false);
          console.log('error');
        });
    } else {
      setIsAuthenticated(false);
    }
  }, []);

  const handleAuthSuccess = (token) => {
    authService.setToken(token);
    localStorage.setItem('token', token);
    getUser();
    console.log(user);
    setIsAuthenticated(true);

    // navigate('/', { replace: true });
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
      console.log(err);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const value = {
    user,
    loading,
    error,
    login,
    register,
    logout,
    isAuthenticated,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
