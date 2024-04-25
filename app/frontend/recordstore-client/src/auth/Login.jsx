// src/components/Login.js
import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import useAuth from './useAuth';

import {
  Center,
  Input,
  Button,
  FormControl,
  Heading,
  FormLabel,
} from '@chakra-ui/react';

const Login = () => {
  const { login, error, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [credentials, setCredentials] = useState({
    email: '',
    password: '',
  });

  useEffect(() => {
    if (isAuthenticated) {
      const { from } = location.state || { from: { pathname: '/' } };
      navigate(from);
    }
  }, [error, isAuthenticated]);

  const handleChange = (e) => {
    setCredentials({ ...credentials, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await login(credentials.email, credentials.password);
  };

  return (
    <Center h='full'>
      <div>
        {error && <p>{error}</p>}
        <Heading>Вхід</Heading>
        <form onSubmit={handleSubmit}>
          <FormControl mt={6} isRequired>
            <FormLabel>Електронна пошта</FormLabel>
            <Input
              type='text'
              name='email'
              placeholder='Email'
              value={credentials.email}
              onChange={handleChange}
            />
          </FormControl>
          <FormControl mt={6} isRequired>
            <FormLabel>Пароль</FormLabel>
            <Input
              type='password'
              name='password'
              placeholder='Password'
              value={credentials.password}
              onChange={handleChange}
            />
          </FormControl>
          <Button type='submit' colorScheme='teal' mt={6} width='full'>
            Увійти
          </Button>
        </form>
      </div>
    </Center>
  );
};

export default Login;
