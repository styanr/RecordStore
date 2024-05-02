// src/components/Register.js

import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import useAuth from '../hooks/useAuth';
import {
  Box,
  Container,
  Input,
  Button,
  FormControl,
  Heading,
  FormLabel,
  Flex,
  useToast,
} from '@chakra-ui/react';

const Register = () => {
  const toast = useToast();
  const { register, error, isAuthenticated, loading } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [credentials, setCredentials] = useState({
    email: '',
    password: '',
    firstName: '',
    lastName: '',
  });

  useEffect(() => {
    if (isAuthenticated && !loading) {
      const { from } = location.state || { from: { pathname: '/' } };
      navigate(from);
    }
  }, [error, isAuthenticated]);

  const handleChange = (e) => {
    setCredentials({ ...credentials, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await register({
      email: credentials.email,
      password: credentials.password,
      firstName: credentials.firstName,
      lastName: credentials.lastName,
    });
  };

  useEffect(() => {
    console.log(error);
    if (error) {
      toast({
        title: 'Помилка реєстрації',
        description: error,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
  }, [error]);

  return (
    <Box
      bg='gray.100'
      py={12}
      flexGrow={1}
      display='flex'
      justifyContent='center'
      alignItems='center'
      flexDir={'column'}
    >
      <Container maxW='xl'>
        <Heading color='teal.600' mb={4}>
          Реєстрація
        </Heading>
        <form onSubmit={handleSubmit}>
          <FormControl mt={6} isRequired>
            <FormLabel>Email</FormLabel>
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
              placeholder='Пароль'
              value={credentials.password}
              onChange={handleChange}
            />
          </FormControl>
          <FormControl mt={6} isRequired>
            <FormLabel>Ім'я</FormLabel>
            <Input
              type='text'
              name='firstName'
              placeholder="Ім'я"
              value={credentials.firstName}
              onChange={handleChange}
            />
          </FormControl>
          <FormControl mt={6} isRequired>
            <FormLabel>Прізвище</FormLabel>
            <Input
              type='text'
              name='lastName'
              placeholder='Прізвище'
              value={credentials.lastName}
              onChange={handleChange}
            />
          </FormControl>
          <Button type='submit' colorScheme='teal' mt={6} width='full'>
            Зареєструватися
          </Button>
        </form>
      </Container>
    </Box>
  );
};

export default Register;
