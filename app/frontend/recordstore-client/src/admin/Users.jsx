import React, { useState, useEffect } from 'react';
import {
  Box,
  Container,
  Heading,
  Spinner,
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  Button,
  Flex,
  IconButton,
  Badge,
  Text,
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalCloseButton,
  ModalBody,
  ModalFooter,
  useDisclosure,
  useToast,
  TableContainer,
} from '@chakra-ui/react';
import {
  FaTrashAlt,
  FaSort,
  FaSortUp,
  FaSortDown,
  FaPlus,
} from 'react-icons/fa';

import { Link } from 'react-router-dom';

import formatDate from '../utils/formatDate';
import useUsers from '../hooks/useUsers';
import usePages from '../hooks/usePages';
import useAuth from '../hooks/useAuth';

const Users = () => {
  const toast = useToast();
  const {
    users,
    loading,
    setParams,
    params,
    roles,
    rolesLoading,
    updateRole,
    deleteUser,
  } = useUsers();
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [userToDelete, setUserToDelete] = useState(null);
  const { page, totalPages, setTotalPages, nextPage, prevPage, goToPage } =
    usePages();
  const [isDeleteLoading, setIsDeleteLoading] = useState(false);
  const [orderBy, setOrderBy] = useState('createdAt');
  const [orderDirection, setOrderDirection] = useState('desc');

  const [isRoleModalOpen, setIsRoleModalOpen] = useState(false);
  const [userToChangeRole, setUserToChangeRole] = useState(null);

  const { user } = useAuth();

  useEffect(() => {
    setTotalPages(users.pageCount);
  }, [users.pageCount]);

  useEffect(() => {
    setParams((prevParams) => ({
      ...prevParams,
      page,
      orderBy,
      orderDirection,
    }));
  }, [page, orderBy, orderDirection]);

  const handleSortChange = (column) => {
    if (orderBy === column) {
      setOrderDirection(orderDirection === 'asc' ? 'desc' : 'asc');
    } else {
      setOrderBy(column);
      setOrderDirection('asc');
    }
  };

  const renderSortIcon = (column) => {
    if (orderBy !== column) {
      return <FaSort />;
    } else if (orderDirection === 'asc') {
      return <FaSortUp />;
    } else {
      return <FaSortDown />;
    }
  };

  const handleChangeRoleClick = (user) => {
    setUserToChangeRole(user);
    setIsRoleModalOpen(true);
  };

  const handleDeleteClick = (user) => {
    setUserToDelete(user);
    onOpen();
  };

  const handleChangeRole = async (newRole) => {
    console.log(`Changing role of ${userToChangeRole.id} to ${newRole}`);

    const response = await updateRole(userToChangeRole.id, newRole);

    if (response.success) {
      toast({
        title: 'Роль користувача успішно змінена',
        status: 'success',
        duration: 5000,
        isClosable: true,
      });
    } else {
      toast({
        title: 'Помилка зміни ролі користувача',
        description: response.error,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
    setIsRoleModalOpen(false);
    setUserToChangeRole(null);
  };

  const handleDelete = async () => {
    setIsDeleteLoading(true);
    const response = await deleteUser(userToDelete.id);
    if (response.success) {
      toast({
        title: 'Користувача успішно видалено',
        status: 'success',
        duration: 5000,
        isClosable: true,
      });
    } else {
      toast({
        title: 'Помилка видалення користувача',
        description: response.error,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
    onClose();
    setIsDeleteLoading(false);
  };

  return (
    <>
      <Heading as='h3' size='lg' mt='5' mb='4'>
        Користувачі
      </Heading>
      <Button
        colorScheme='teal'
        leftIcon={<FaPlus />}
        _hover={{ bg: 'teal.700' }}
        transition='background-color 0.3s ease'
        as={Link}
        to='/users/new'
        maxW='fit-content'
      >
        Створити користувача
      </Button>
      <TableContainer
        boxShadow='md'
        borderRadius='lg'
        overflow='hidden'
        backgroundColor='white'
        mt={4}
      >
        <Table variant='simple'>
          <Thead>
            <Tr>
              <Th onClick={() => handleSortChange('id')}>
                <Flex align='center'>
                  Ідентифікатор
                  {renderSortIcon('id')}
                </Flex>
              </Th>
              <Th onClick={() => handleSortChange('name')}>
                <Flex align='center'>
                  Ім'я
                  {renderSortIcon('name')}
                </Flex>
              </Th>
              <Th onClick={() => handleSortChange('email')}>
                <Flex align='center'>
                  Email
                  {renderSortIcon('email')}
                </Flex>
              </Th>
              <Th onClick={() => handleSortChange('role')}>
                <Flex align='center'>
                  Роль
                  {renderSortIcon('role')}
                </Flex>
              </Th>
              <Th onClick={() => handleSortChange('createdAt')}>
                <Flex align='center'>
                  Дата створення
                  {renderSortIcon('createdAt')}
                </Flex>
              </Th>
              <Th>Дії</Th>
            </Tr>
          </Thead>

          {loading ? (
            <Box
              display='flex'
              justifyContent='center'
              alignItems='center'
              flexGrow={1}
              w='100%'
            >
              <Spinner
                size='xl'
                color='teal.500'
                thickness='4px'
                speed='0.65s'
                emptyColor='gray.200'
              />
            </Box>
          ) : (
            <Tbody>
              {users.results.map((user) => (
                <User
                  key={user.id}
                  user={user}
                  onDelete={handleDeleteClick}
                  onRoleChange={handleChangeRoleClick}
                />
              ))}
            </Tbody>
          )}
        </Table>
      </TableContainer>
      <Flex mt={4} justify='center'>
        <Button isDisabled={page === 1} onClick={() => prevPage()} size='sm'>
          Попередня
        </Button>
        <Text mx={2}>{page}</Text>
        <Button
          isDisabled={page === totalPages}
          onClick={() => nextPage()}
          size='sm'
        >
          Наступна
        </Button>
      </Flex>

      <Modal
        isOpen={isRoleModalOpen}
        onClose={() => setIsRoleModalOpen(false)}
        isCentered
      >
        <ModalOverlay
          bg='blackAlpha.300'
          backdropFilter='blur(4px) hue-rotate(90deg)'
          transition='all 0.3s ease'
        />
        <ModalContent>
          <ModalHeader color='teal.600'>Change User Role</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <Box textAlign='center' display={'flex'} flexDirection={'row'}>
              <Box>
                <Heading size='md' mb={2}>
                  Змінити роль для користувача {userToChangeRole?.firstName}{' '}
                  {userToChangeRole?.lastName}
                </Heading>
                {user.id === userToChangeRole?.id && (
                  <Text color='red.500' as='b'>
                    Ви змінюєте свою власну роль. Це може призвести до втрати
                    доступу до деяких функцій або даних.
                  </Text>
                )}
                <Text>Оберіть нову роль для користувача:</Text>
                {rolesLoading ? (
                  <Spinner />
                ) : (
                  <Box display='flex' flexDirection='row' mt={4}>
                    {roles.map((role) => (
                      <Button
                        key={role.id}
                        colorScheme='teal'
                        variant='outline'
                        size='sm'
                        onClick={() => handleChangeRole(role.id)}
                        mr={2}
                      >
                        {role.name}
                      </Button>
                    ))}
                  </Box>
                )}
              </Box>
            </Box>
          </ModalBody>
          <ModalFooter>
            <Button
              variant='ghost'
              mr={3}
              onClick={() => setIsRoleModalOpen(false)}
            >
              Скасувати
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
      <Modal isOpen={isOpen} onClose={onClose} isCentered>
        <ModalOverlay
          bg='blackAlpha.300'
          backdropFilter='blur(4px) hue-rotate(90deg)'
          transition='all 0.3s ease'
        />
        <ModalContent>
          <ModalHeader color='teal.600'>Підтвердження видалення</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <Box textAlign='center' display={'flex'} flexDirection={'row'}>
              <FaTrashAlt size={48} color='red.500' mb={4} />
              <Box>
                <Heading size='md' mb={2}>
                  Ви впевнені, що хочете видалити користувача{' '}
                  {userToDelete?.firstName} {userToDelete?.lastName}?
                </Heading>
                {user.id === userToDelete?.id && (
                  <Text color='red.500' as='b'>
                    Ви видаляєте свій власний акаунт. Це призведе до видалення
                    всієї вашої інформації та виходу з системи.
                  </Text>
                )}
                <Text>Цю дію неможливо скасувати.</Text>
              </Box>
            </Box>
          </ModalBody>
          <ModalFooter>
            <Button variant='ghost' mr={3} onClick={onClose}>
              Cancel
            </Button>
            <Button
              colorScheme='red'
              onClick={handleDelete}
              _hover={{ bg: 'red.600' }}
              transition='background-color 0.3s ease'
              isLoading={isDeleteLoading}
            >
              Delete
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
};

import stringToColor from '../utils/stringToColor';
import CustomBadge from '../profile/CustomBadge';

const User = ({ user, onDelete, onRoleChange }) => {
  const handleDeleteClick = () => {
    onDelete(user);
  };

  return (
    <>
      <Tr
        key={user.id}
        _hover={{ bg: 'teal.50' }}
        transition='background-color 0.3s ease, transform 0.3s ease'
      >
        <Td>
          <Badge colorScheme='teal' mr={2}>
            {user.id}
          </Badge>
        </Td>
        <Td>
          {user.firstName} {user.lastName}
        </Td>
        <Td>{user.email}</Td>
        <Td>
          <CustomBadge
            color={stringToColor(user.role)}
            onClick={() => onRoleChange(user)}
            cursor='pointer'
          >
            {user.role}
          </CustomBadge>
        </Td>
        <Td>{formatDate(user.createdAt)}</Td>
        <Td>
          <IconButton
            aria-label='Delete'
            icon={<FaTrashAlt />}
            colorScheme='red'
            variant='ghost'
            size='sm'
            onClick={(e) => {
              e.stopPropagation();
              handleDeleteClick();
            }}
          />
        </Td>
      </Tr>
    </>
  );
};

export default Users;
