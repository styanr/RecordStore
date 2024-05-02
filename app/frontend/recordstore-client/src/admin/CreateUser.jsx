import React, { useState, useEffect } from 'react';
import {
  Box,
  Button,
  Flex,
  FormControl,
  FormLabel,
  Heading,
  Input,
  Select,
  useToast,
} from '@chakra-ui/react';

import useUsers from '../hooks/useUsers';
import { useNavigate } from 'react-router-dom';

const CreateUser = () => {
  const { createUser, roles } = useUsers();
  const toast = useToast();
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    firstName: '',
    lastName: '',
    roleId: '',
  });
  const [isLoading, setIsLoading] = useState(false);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);

    const { success, error } = await createUser(formData);

    if (success) {
      toast({
        title: 'User created successfully',
        status: 'success',
        duration: 3000,
        isClosable: true,
      });
      navigate('/dashboard');
    } else {
      toast({
        title: 'Error creating user',
        description: error,
        status: 'error',
        duration: 3000,
        isClosable: true,
      });
    }

    setIsLoading(false);
  };

  return (
    <Flex align='center' justify='center' h='100vh' bg='gray.100'>
      <Box
        bg='white'
        p={8}
        borderRadius='md'
        boxShadow='md'
        maxW='500px'
        w='full'
      >
        <Heading color='teal.600' mb={6}>
          Create User
        </Heading>
        <form onSubmit={handleSubmit}>
          <FormControl mb={4}>
            <FormLabel htmlFor='email'>Email</FormLabel>
            <Input
              type='email'
              id='email'
              name='email'
              value={formData.email}
              onChange={handleChange}
              required
            />
          </FormControl>
          <FormControl mb={4}>
            <FormLabel htmlFor='password'>Password</FormLabel>
            <Input
              type='password'
              id='password'
              name='password'
              value={formData.password}
              onChange={handleChange}
              required
            />
          </FormControl>
          <Flex mb={4}>
            <FormControl mr={4}>
              <FormLabel htmlFor='firstName'>First Name</FormLabel>
              <Input
                type='text'
                id='firstName'
                name='firstName'
                value={formData.firstName}
                onChange={handleChange}
                required
              />
            </FormControl>
            <FormControl>
              <FormLabel htmlFor='lastName'>Last Name</FormLabel>
              <Input
                type='text'
                id='lastName'
                name='lastName'
                value={formData.lastName}
                onChange={handleChange}
                required
              />
            </FormControl>
          </Flex>
          <FormControl mb={6}>
            <FormLabel htmlFor='roleId'>Role</FormLabel>
            <Select
              id='roleId'
              name='roleId'
              value={formData.roleId}
              onChange={handleChange}
              required
            >
              <option value=''>Select Role</option>
              {roles.map((role) => (
                <option key={role.id} value={role.id}>
                  {role.name}
                </option>
              ))}
            </Select>
          </FormControl>
          <Button
            colorScheme='teal'
            type='submit'
            isLoading={isLoading}
            loadingText='Creating...'
            w='full'
          >
            Create User
          </Button>
        </form>
      </Box>
    </Flex>
  );
};

export default CreateUser;
